using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using SmartGreenhouse.Models.Entities;

namespace SmartGreenhouse.DAL.Repositories
{
    /// <summary>
    /// Repository pentru operații cu utilizatori
    /// </summary>
    public class UserRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public UserRepository()
        {
            _dbHelper = new DatabaseHelper();
        }

        /// <summary>
        /// Autentifică un utilizator
        /// </summary>
        public User Authenticate(string username, string passwordHash, string plainPassword = null)
        {
            // Primul încercăm cu hash (standard)
            var user = AuthenticateInternal(username, passwordHash);
            if (user != null)
                return user;

            // Dacă nu găsim utilizator, încercăm un fallback pe parolă simplă (pentru baze vechi)
            if (plainPassword == null)
                return null;

            var userPlain = AuthenticateInternal(username, passwordHash, allowPlainPassword: true, plainTextPassword: plainPassword);
            if (userPlain != null)
            {
                // Migrare: actualizăm parola la hash pentru următoarea autentificare
                try
                {
                    UpdatePassword(userPlain.UserId, passwordHash);
                }
                catch
                {
                    // Ignore, nu este critic
                }
            }

            return userPlain;
        }

        private User AuthenticateInternal(string username, string passwordHash, bool allowPlainPassword = false, string plainTextPassword = null)
        {
            string query = @"
                SELECT UserId, Username, Email, Role, IsActive, LastLogin
                FROM Users
                WHERE Username = @Username AND IsActive = 1" + (allowPlainPassword ? " AND (PasswordHash = @PasswordHash OR PasswordHash = @PlainText)" : " AND PasswordHash = @PasswordHash");

            var parameters = new[]
            {
                new SqlParameter("@Username", username),
                new SqlParameter("@PasswordHash", passwordHash),
                new SqlParameter("@PlainText", plainTextPassword ?? string.Empty)
            };

            var result = _dbHelper.ExecuteQuery(query, parameters);

            if (result.Rows.Count == 0)
                return null;

            var row = result.Rows[0];
            
            // Actualizăm ultima autentificare
            UpdateLastLogin(Convert.ToInt32(row["UserId"]));

            return new User
            {
                UserId = Convert.ToInt32(row["UserId"]),
                Username = row["Username"].ToString(),
                Email = row["Email"].ToString(),
                Role = (UserRole)Convert.ToInt32(row["Role"]),
                IsActive = Convert.ToBoolean(row["IsActive"]),
                LastLogin = row["LastLogin"] != DBNull.Value ? Convert.ToDateTime(row["LastLogin"]) : (DateTime?)null
            };
        }

        public bool UpdatePassword(int userId, string newPasswordHash)
        {
            string query = "UPDATE Users SET PasswordHash = @PasswordHash WHERE UserId = @UserId";
            var parameters = new[]
            {
                new SqlParameter("@PasswordHash", newPasswordHash),
                new SqlParameter("@UserId", userId)
            };

            return _dbHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool UpdateUser(User user)
        {
            string query = @"
                UPDATE Users
                SET Username = @Username,
                    Email = @Email,
                    Role = @Role,
                    IsActive = @IsActive
                WHERE UserId = @UserId";

            var parameters = new[]
            {
                new SqlParameter("@Username", user.Username),
                new SqlParameter("@Email", user.Email),
                new SqlParameter("@Role", (int)user.Role),
                new SqlParameter("@IsActive", user.IsActive),
                new SqlParameter("@UserId", user.UserId)
            };

            return _dbHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool DeleteUser(int userId)
        {
            string query = "UPDATE Users SET IsActive = 0 WHERE UserId = @UserId";
            var parameter = new SqlParameter("@UserId", userId);
            return _dbHelper.ExecuteNonQuery(query, parameter) > 0;
        }

        /// <summary>
        /// Actualizează ultima autentificare
        /// </summary>
        private void UpdateLastLogin(int userId)
        {
            string query = "UPDATE Users SET LastLogin = GETDATE() WHERE UserId = @UserId";
            var parameter = new SqlParameter("@UserId", userId);
            _dbHelper.ExecuteNonQuery(query, parameter);
        }

        /// <summary>
        /// Obține toți utilizatorii
        /// </summary>
        public List<User> GetAllUsers()
        {
            string query = "SELECT UserId, Username, Email, Role, IsActive, CreatedAt, LastLogin FROM Users";
            var result = _dbHelper.ExecuteQuery(query);
            
            var users = new List<User>();
            
            foreach (DataRow row in result.Rows)
            {
                users.Add(new User
                {
                    UserId = Convert.ToInt32(row["UserId"]),
                    Username = row["Username"].ToString(),
                    Email = row["Email"].ToString(),
                    Role = (UserRole)Convert.ToInt32(row["Role"]),
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                    LastLogin = row["LastLogin"] != DBNull.Value ? Convert.ToDateTime(row["LastLogin"]) : (DateTime?)null
                });
            }
            
            return users;
        }

        /// <summary>
        /// Adaugă un utilizator nou
        /// </summary>
        public bool AddUser(User user, string passwordHash)
        {
            string query = @"
                INSERT INTO Users (Username, PasswordHash, Email, Role, CreatedAt, IsActive)
                VALUES (@Username, @PasswordHash, @Email, @Role, @CreatedAt, @IsActive)";

            var parameters = new[]
            {
                new SqlParameter("@Username", user.Username),
                new SqlParameter("@PasswordHash", passwordHash),
                new SqlParameter("@Email", user.Email),
                new SqlParameter("@Role", (int)user.Role),
                new SqlParameter("@CreatedAt", DateTime.Now),
                new SqlParameter("@IsActive", user.IsActive)
            };

            return _dbHelper.ExecuteNonQuery(query, parameters) > 0;
        }
    }
}
using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace SmartGreenhouse.DAL
{
    /// <summary>
    /// Clasă helper pentru conexiunea la baza de date
    /// </summary>
    public class DatabaseHelper : IDisposable
    {
        private SqlConnection _connection;
        private readonly string _connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=SmartGreenhouseDB;Integrated Security=True;TrustServerCertificate=True";

        /// <summary>
        /// Returnează string-ul de conexiune
        /// </summary>
        public static string GetConnectionString()
        {
            return "Data Source=localhost\\SQLEXPRESS;Initial Catalog=SmartGreenhouseDB;Integrated Security=True;TrustServerCertificate=True";
        }

        /// <summary>
        /// Deschide conexiunea la baza de date
        /// </summary>
        public SqlConnection GetConnection()
        {
            if (_connection == null)
            {
                _connection = new SqlConnection(_connectionString);
            }

            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            return _connection;
        }

        /// <summary>
        /// Execută o comandă SQL și returnează un DataTable
        /// </summary>
        public DataTable ExecuteQuery(string query, params SqlParameter[] parameters)
        {
            using (var command = new SqlCommand(query, GetConnection()))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                using (var adapter = new SqlDataAdapter(command))
                {
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }

        /// <summary>
        /// Execută o comandă SQL și returnează numărul de rânduri afectate
        /// </summary>
        public int ExecuteNonQuery(string query, params SqlParameter[] parameters)
        {
            using (var command = new SqlCommand(query, GetConnection()))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Execută o comandă SQL și returnează prima valoare
        /// </summary>
        public object ExecuteScalar(string query, params SqlParameter[] parameters)
        {
            using (var command = new SqlCommand(query, GetConnection()))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                return command.ExecuteScalar();
            }
        }

        public void Dispose()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                _connection.Close();
                _connection.Dispose();
            }
        }
    }
}
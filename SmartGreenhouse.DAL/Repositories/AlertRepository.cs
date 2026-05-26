using System;
using System.Collections.Generic;
using System.Linq;
using SmartGreenhouse.Models.Entities;
using Microsoft.Data.SqlClient;

namespace SmartGreenhouse.DAL.Repositories
{
    public class AlertRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public AlertRepository()
        {
            _dbHelper = new DatabaseHelper();
        }

        public List<Alert> GetAllAlerts()
        {
            var alerts = new List<Alert>();
            string query = @"
                SELECT a.AlertId, a.SensorId, a.ActuatorId, a.Timestamp, a.Level, a.Message, a.Value,
                       a.IsAcknowledged, a.AcknowledgedByUserId, a.AcknowledgedAt,
                       s.Name as SensorName, u.Username as AcknowledgedByUsername
                FROM Alerts a
                LEFT JOIN Sensors s ON a.SensorId = s.SensorId
                LEFT JOIN Users u ON a.AcknowledgedByUserId = u.UserId
                ORDER BY a.Timestamp DESC";

            var dataTable = _dbHelper.ExecuteQuery(query);

            foreach (System.Data.DataRow row in dataTable.Rows)
            {
                var alert = new Alert
                {
                    AlertId = Convert.ToInt32(row["AlertId"]),
                    SensorId = row["SensorId"] != DBNull.Value ? Convert.ToInt32(row["SensorId"]) : (int?)null,
                    ActuatorId = row["ActuatorId"] != DBNull.Value ? Convert.ToInt32(row["ActuatorId"]) : (int?)null,
                    Timestamp = Convert.ToDateTime(row["Timestamp"]),
                    Level = (AlertLevel)Convert.ToInt32(row["Level"]),
                    Message = row["Message"].ToString(),
                    Value = row["Value"] != DBNull.Value ? Convert.ToDouble(row["Value"]) : (double?)null,
                    IsAcknowledged = Convert.ToBoolean(row["IsAcknowledged"]),
                    AcknowledgedByUserId = row["AcknowledgedByUserId"] != DBNull.Value ? Convert.ToInt32(row["AcknowledgedByUserId"]) : (int?)null,
                    AcknowledgedAt = row["AcknowledgedAt"] != DBNull.Value ? Convert.ToDateTime(row["AcknowledgedAt"]) : (DateTime?)null
                };

                alerts.Add(alert);
            }

            return alerts;
        }

        public Alert GetAlertById(int alertId)
        {
            string query = @"
                SELECT a.AlertId, a.SensorId, a.ActuatorId, a.Timestamp, a.Level, a.Message, a.Value,
                       a.IsAcknowledged, a.AcknowledgedByUserId, a.AcknowledgedAt
                FROM Alerts a
                WHERE a.AlertId = @AlertId";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@AlertId", alertId)
            };

            var dataTable = _dbHelper.ExecuteQuery(query, parameters);

            if (dataTable.Rows.Count == 0)
                return null;

            var row = dataTable.Rows[0];
            return new Alert
            {
                AlertId = Convert.ToInt32(row["AlertId"]),
                SensorId = row["SensorId"] != DBNull.Value ? Convert.ToInt32(row["SensorId"]) : (int?)null,
                ActuatorId = row["ActuatorId"] != DBNull.Value ? Convert.ToInt32(row["ActuatorId"]) : (int?)null,
                Timestamp = Convert.ToDateTime(row["Timestamp"]),
                Level = (AlertLevel)Convert.ToInt32(row["Level"]),
                Message = row["Message"].ToString(),
                Value = row["Value"] != DBNull.Value ? Convert.ToDouble(row["Value"]) : (double?)null,
                IsAcknowledged = Convert.ToBoolean(row["IsAcknowledged"]),
                AcknowledgedByUserId = row["AcknowledgedByUserId"] != DBNull.Value ? Convert.ToInt32(row["AcknowledgedByUserId"]) : (int?)null,
                AcknowledgedAt = row["AcknowledgedAt"] != DBNull.Value ? Convert.ToDateTime(row["AcknowledgedAt"]) : (DateTime?)null
            };
        }

        public void AddAlert(Alert alert)
        {
            string query = @"
                INSERT INTO Alerts (SensorId, ActuatorId, Timestamp, Level, Message, Value, IsAcknowledged)
                VALUES (@SensorId, @ActuatorId, @Timestamp, @Level, @Message, @Value, @IsAcknowledged)";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@SensorId", alert.SensorId ?? (object)DBNull.Value),
                new SqlParameter("@ActuatorId", alert.ActuatorId ?? (object)DBNull.Value),
                new SqlParameter("@Timestamp", alert.Timestamp),
                new SqlParameter("@Level", (int)alert.Level),
                new SqlParameter("@Message", alert.Message),
                new SqlParameter("@Value", alert.Value ?? (object)DBNull.Value),
                new SqlParameter("@IsAcknowledged", alert.IsAcknowledged)
            };

            _dbHelper.ExecuteNonQuery(query, parameters);
        }

        public void UpdateAlert(Alert alert)
        {
            string query = @"
                UPDATE Alerts
                SET SensorId = @SensorId, ActuatorId = @ActuatorId, Timestamp = @Timestamp,
                    Level = @Level, Message = @Message, Value = @Value, IsAcknowledged = @IsAcknowledged,
                    AcknowledgedByUserId = @AcknowledgedByUserId, AcknowledgedAt = @AcknowledgedAt
                WHERE AlertId = @AlertId";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@AlertId", alert.AlertId),
                new SqlParameter("@SensorId", alert.SensorId ?? (object)DBNull.Value),
                new SqlParameter("@ActuatorId", alert.ActuatorId ?? (object)DBNull.Value),
                new SqlParameter("@Timestamp", alert.Timestamp),
                new SqlParameter("@Level", (int)alert.Level),
                new SqlParameter("@Message", alert.Message),
                new SqlParameter("@Value", alert.Value ?? (object)DBNull.Value),
                new SqlParameter("@IsAcknowledged", alert.IsAcknowledged),
                new SqlParameter("@AcknowledgedByUserId", alert.AcknowledgedByUserId ?? (object)DBNull.Value),
                new SqlParameter("@AcknowledgedAt", alert.AcknowledgedAt ?? (object)DBNull.Value)
            };

            _dbHelper.ExecuteNonQuery(query, parameters);
        }

        public void DeleteAlert(int alertId)
        {
            string query = "DELETE FROM Alerts WHERE AlertId = @AlertId";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@AlertId", alertId)
            };

            _dbHelper.ExecuteNonQuery(query, parameters);
        }

        public void AcknowledgeAlert(int alertId, int userId)
        {
            string query = @"
                UPDATE Alerts
                SET IsAcknowledged = 1, AcknowledgedByUserId = @UserId, AcknowledgedAt = @Timestamp
                WHERE AlertId = @AlertId";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@AlertId", alertId),
                new SqlParameter("@UserId", userId),
                new SqlParameter("@Timestamp", DateTime.Now)
            };

            _dbHelper.ExecuteNonQuery(query, parameters);
        }

        public List<Alert> GetUnacknowledgedAlerts()
        {
            return GetAllAlerts().Where(a => !a.IsAcknowledged).ToList();
        }

        public List<Alert> GetAlertsBySensor(int sensorId)
        {
            return GetAllAlerts().Where(a => a.SensorId == sensorId).ToList();
        }
    }
}
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using SmartGreenhouse.DAL;
using SmartGreenhouse.Models.Entities;

namespace SmartGreenhouse.BLL.Services
{
    /// <summary>
    /// Delegate pentru evenimentele de alertă
    /// </summary>
    public delegate void AlertEventHandler(object sender, AlertEventArgs e);

    /// <summary>
    /// Clasa pentru argumentele evenimentului de alertă
    /// </summary>
    public class AlertEventArgs : EventArgs
    {
        public Alert Alert { get; set; }
        public string SensorName { get; set; }
        public string Location { get; set; }
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Serviciu pentru gestionarea alertelor
    /// </summary>
    public class AlertService
    {
        private readonly DatabaseHelper _dbHelper;
        
        // Evenimente WOW!
        public event AlertEventHandler AlertGenerated;
        public event EventHandler CriticalAlertGenerated;

        public AlertService()
        {
            _dbHelper = new DatabaseHelper();
        }

        /// <summary>
        /// Verifică dacă o citire depășește pragurile și generează alerte
        /// </summary>
        public void CheckThresholds(Sensor sensor, SensorReading reading)
        {
            var alerts = new List<Alert>();

            // Verifică pragurile critice
            if (reading.Value >= sensor.CriticalHigh)
            {
                var alert = CreateAlert(sensor, reading, AlertLevel.Critical, 
                    $"CRITICAL: {sensor.Name} a depășit pragul superior! Valoare: {reading.Value}{sensor.Unit}");
                alerts.Add(alert);
            }
            else if (reading.Value <= sensor.CriticalLow)
            {
                var alert = CreateAlert(sensor, reading, AlertLevel.Critical, 
                    $"CRITICAL: {sensor.Name} a scăzut sub pragul inferior! Valoare: {reading.Value}{sensor.Unit}");
                alerts.Add(alert);
            }
            // Verifică pragurile de avertizare
            else if (reading.Value >= sensor.WarningHigh)
            {
                var alert = CreateAlert(sensor, reading, AlertLevel.Warning, 
                    $"WARNING: {sensor.Name} aproape de pragul superior! Valoare: {reading.Value}{sensor.Unit}");
                alerts.Add(alert);
            }
            else if (reading.Value <= sensor.WarningLow)
            {
                var alert = CreateAlert(sensor, reading, AlertLevel.Warning, 
                    $"WARNING: {sensor.Name} aproape de pragul inferior! Valoare: {reading.Value}{sensor.Unit}");
                alerts.Add(alert);
            }

            // Salvează și declanșează evenimente pentru fiecare alertă
            foreach (var alert in alerts)
            {
                SaveAlert(alert);
                OnAlertGenerated(alert, sensor);
            }
        }

        /// <summary>
        /// Creează un obiect Alert
        /// </summary>
        private Alert CreateAlert(Sensor sensor, SensorReading reading, AlertLevel level, string message)
        {
            return new Alert
            {
                SensorId = sensor.SensorId,
                Timestamp = reading.Timestamp,
                Level = level,
                Message = message,
                Value = reading.Value,
                IsAcknowledged = false
            };
        }

        /// <summary>
        /// Salvează alerta în baza de date
        /// </summary>
        private void SaveAlert(Alert alert)
        {
            string query = @"
                INSERT INTO Alerts (SensorId, Timestamp, Level, Message, Value, IsAcknowledged)
                VALUES (@SensorId, @Timestamp, @Level, @Message, @Value, @IsAcknowledged)";

            var parameters = new[]
            {
                new SqlParameter("@SensorId", alert.SensorId),
                new SqlParameter("@Timestamp", alert.Timestamp),
                new SqlParameter("@Level", (int)alert.Level),
                new SqlParameter("@Message", alert.Message),
                new SqlParameter("@Value", alert.Value),
                new SqlParameter("@IsAcknowledged", alert.IsAcknowledged)
            };

            _dbHelper.ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// Declanșează evenimentul de alertă
        /// </summary>
        protected virtual void OnAlertGenerated(Alert alert, Sensor sensor)
        {
            AlertGenerated?.Invoke(this, new AlertEventArgs
            {
                Alert = alert,
                SensorName = sensor?.Name,
                Location = sensor?.Location,
                Timestamp = alert.Timestamp
            });

            if (alert.Level == AlertLevel.Critical)
            {
                CriticalAlertGenerated?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Obține alertele neconfirmate
        /// </summary>
        public List<Alert> GetUnacknowledgedAlerts()
        {
            string query = @"
                SELECT a.*, s.Name as SensorName, s.Location
                FROM Alerts a
                LEFT JOIN Sensors s ON a.SensorId = s.SensorId
                WHERE a.IsAcknowledged = 0
                ORDER BY a.Timestamp DESC";

            var result = _dbHelper.ExecuteQuery(query);
            var alerts = new List<Alert>();

            foreach (System.Data.DataRow row in result.Rows)
            {
                alerts.Add(new Alert
                {
                    AlertId = Convert.ToInt32(row["AlertId"]),
                    SensorId = row["SensorId"] != DBNull.Value ? Convert.ToInt32(row["SensorId"]) : (int?)null,
                    Timestamp = Convert.ToDateTime(row["Timestamp"]),
                    Level = (AlertLevel)Convert.ToInt32(row["Level"]),
                    Message = row["Message"].ToString(),
                    Value = row["Value"] != DBNull.Value ? Convert.ToDouble(row["Value"]) : (double?)null,
                    IsAcknowledged = Convert.ToBoolean(row["IsAcknowledged"])
                });
            }

            return alerts;
        }

        /// <summary>
        /// Confirmă o alertă
        /// </summary>
        public bool AcknowledgeAlert(int alertId, int userId)
        {
            string query = @"
                UPDATE Alerts 
                SET IsAcknowledged = 1, AcknowledgedByUserId = @UserId, AcknowledgedAt = GETDATE()
                WHERE AlertId = @AlertId";

            var parameters = new[]
            {
                new SqlParameter("@AlertId", alertId),
                new SqlParameter("@UserId", userId)
            };

            return _dbHelper.ExecuteNonQuery(query, parameters) > 0;
        }
    }
}
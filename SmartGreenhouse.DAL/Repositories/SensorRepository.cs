using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using SmartGreenhouse.Models.Entities;

namespace SmartGreenhouse.DAL.Repositories
{
    /// <summary>
    /// Repository pentru operații cu senzori
    /// </summary>
    public class SensorRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public SensorRepository()
        {
            _dbHelper = new DatabaseHelper();
        }

        /// <summary>
        /// Obține toți senzorii activi
        /// </summary>
        public List<Sensor> GetAllSensors()
        {
            string query = @"
                SELECT s.*, u.Username as CreatedByUsername
                FROM Sensors s
                LEFT JOIN Users u ON s.CreatedByUserId = u.UserId
                WHERE s.IsActive = 1
                ORDER BY s.SensorId";

            var result = _dbHelper.ExecuteQuery(query);
            var sensors = new List<Sensor>();

            foreach (DataRow row in result.Rows)
            {
                sensors.Add(MapToSensor(row));
            }

            return sensors;
        }

        /// <summary>
        /// Obține un senzor după ID
        /// </summary>
        public Sensor GetSensorById(int sensorId)
        {
            string query = "SELECT * FROM Sensors WHERE SensorId = @SensorId";
            var parameter = new SqlParameter("@SensorId", sensorId);
            
            var result = _dbHelper.ExecuteQuery(query, parameter);
            
            if (result.Rows.Count == 0)
                return null;

            return MapToSensor(result.Rows[0]);
        }

        /// <summary>
        /// Adaugă un senzor nou
        /// </summary>
        public bool AddSensor(Sensor sensor)
        {
            string query = @"
                INSERT INTO Sensors (Name, Type, Unit, Location, MinValue, MaxValue, 
                                    WarningLow, WarningHigh, CriticalLow, CriticalHigh, 
                                    IsActive, InstalledDate, CreatedByUserId)
                VALUES (@Name, @Type, @Unit, @Location, @MinValue, @MaxValue, 
                        @WarningLow, @WarningHigh, @CriticalLow, @CriticalHigh, 
                        @IsActive, @InstalledDate, @CreatedByUserId)";

            var parameters = new[]
            {
                new SqlParameter("@Name", sensor.Name),
                new SqlParameter("@Type", (int)sensor.Type),
                new SqlParameter("@Unit", sensor.Unit),
                new SqlParameter("@Location", sensor.Location),
                new SqlParameter("@MinValue", sensor.MinValue),
                new SqlParameter("@MaxValue", sensor.MaxValue),
                new SqlParameter("@WarningLow", sensor.WarningLow),
                new SqlParameter("@WarningHigh", sensor.WarningHigh),
                new SqlParameter("@CriticalLow", sensor.CriticalLow),
                new SqlParameter("@CriticalHigh", sensor.CriticalHigh),
                new SqlParameter("@IsActive", sensor.IsActive),
                new SqlParameter("@InstalledDate", sensor.InstalledDate),
                new SqlParameter("@CreatedByUserId", sensor.CreatedByUserId)
            };

            return _dbHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// Actualizează un senzor
        /// </summary>
        public bool UpdateSensor(Sensor sensor)
        {
            string query = @"
                UPDATE Sensors 
                SET Name = @Name, Type = @Type, Unit = @Unit, Location = @Location,
                    MinValue = @MinValue, MaxValue = @MaxValue,
                    WarningLow = @WarningLow, WarningHigh = @WarningHigh,
                    CriticalLow = @CriticalLow, CriticalHigh = @CriticalHigh
                WHERE SensorId = @SensorId";

            var parameters = new[]
            {
                new SqlParameter("@SensorId", sensor.SensorId),
                new SqlParameter("@Name", sensor.Name),
                new SqlParameter("@Type", (int)sensor.Type),
                new SqlParameter("@Unit", sensor.Unit),
                new SqlParameter("@Location", sensor.Location),
                new SqlParameter("@MinValue", sensor.MinValue),
                new SqlParameter("@MaxValue", sensor.MaxValue),
                new SqlParameter("@WarningLow", sensor.WarningLow),
                new SqlParameter("@WarningHigh", sensor.WarningHigh),
                new SqlParameter("@CriticalLow", sensor.CriticalLow),
                new SqlParameter("@CriticalHigh", sensor.CriticalHigh)
            };

            return _dbHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// Șterge un senzor (soft delete)
        /// </summary>
        public bool DeleteSensor(int sensorId)
        {
            string query = "UPDATE Sensors SET IsActive = 0 WHERE SensorId = @SensorId";
            var parameter = new SqlParameter("@SensorId", sensorId);
            
            return _dbHelper.ExecuteNonQuery(query, parameter) > 0;
        }

        /// <summary>
        /// Salvează o citire de la senzor
        /// </summary>
        public bool AddReading(SensorReading reading)
        {
            string query = @"
                INSERT INTO SensorReadings (SensorId, Timestamp, Value, Quality)
                VALUES (@SensorId, @Timestamp, @Value, @Quality)";

            var parameters = new[]
            {
                new SqlParameter("@SensorId", reading.SensorId),
                new SqlParameter("@Timestamp", reading.Timestamp),
                new SqlParameter("@Value", reading.Value),
                new SqlParameter("@Quality", (int)reading.Quality)
            };

            return _dbHelper.ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// Obține ultimele citiri pentru toți senzorii
        /// </summary>
        public DataTable GetLatestReadings()
        {
            string query = @"
                SELECT s.SensorId, s.Name, s.Type, s.Unit, s.Location,
                       r.Value, r.Timestamp, r.Quality
                FROM Sensors s
                OUTER APPLY (
                    SELECT TOP 1 Value, Timestamp, Quality
                    FROM SensorReadings
                    WHERE SensorId = s.SensorId
                    ORDER BY Timestamp DESC
                ) r
                WHERE s.IsActive = 1
                ORDER BY s.Type, s.Name";

            return _dbHelper.ExecuteQuery(query);
        }

        /// <summary>
        /// Obține istoricul citirilor pentru un senzor
        /// </summary>
        public DataTable GetReadingsHistory(int sensorId, DateTime startDate, DateTime endDate)
        {
            string query = @"
                SELECT Timestamp, Value, Quality
                FROM SensorReadings
                WHERE SensorId = @SensorId 
                    AND Timestamp BETWEEN @StartDate AND @EndDate
                ORDER BY Timestamp";

            var parameters = new[]
            {
                new SqlParameter("@SensorId", sensorId),
                new SqlParameter("@StartDate", startDate),
                new SqlParameter("@EndDate", endDate)
            };

            return _dbHelper.ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// Mapează un DataRow la un obiect Sensor
        /// </summary>
        private Sensor MapToSensor(DataRow row)
        {
            return new Sensor
            {
                SensorId = Convert.ToInt32(row["SensorId"]),
                Name = row["Name"].ToString(),
                Type = (SensorType)Convert.ToInt32(row["Type"]),
                Unit = row["Unit"].ToString(),
                Location = row["Location"].ToString(),
                MinValue = Convert.ToDouble(row["MinValue"]),
                MaxValue = Convert.ToDouble(row["MaxValue"]),
                WarningLow = Convert.ToDouble(row["WarningLow"]),
                WarningHigh = Convert.ToDouble(row["WarningHigh"]),
                CriticalLow = Convert.ToDouble(row["CriticalLow"]),
                CriticalHigh = Convert.ToDouble(row["CriticalHigh"]),
                IsActive = Convert.ToBoolean(row["IsActive"]),
                InstalledDate = Convert.ToDateTime(row["InstalledDate"]),
                CreatedByUserId = Convert.ToInt32(row["CreatedByUserId"])
            };
        }
    }
}
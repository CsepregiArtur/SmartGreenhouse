using System;
using System.Collections.Generic;

namespace SmartGreenhouse.Models.Entities
{
    /// <summary>
    /// Reprezintă un senzor din seră
    /// </summary>
    public class Sensor
    {
        public int SensorId { get; set; }
        public string Name { get; set; }
        public SensorType Type { get; set; }
        public string Unit { get; set; }
        public string Location { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public double WarningLow { get; set; }
        public double WarningHigh { get; set; }
        public double CriticalLow { get; set; }
        public double CriticalHigh { get; set; }
        public bool IsActive { get; set; }
        public DateTime InstalledDate { get; set; }
        public int CreatedByUserId { get; set; }
        
        // Navigation properties
        public virtual User CreatedBy { get; set; }
        public virtual ICollection<SensorReading> Readings { get; set; }
        public virtual ICollection<Alert> Alerts { get; set; }
    }

    public enum SensorType
    {
        Temperature = 1,
        Humidity = 2,
        SoilMoisture = 3,
        Light = 4,
        CO2 = 5
    }
}
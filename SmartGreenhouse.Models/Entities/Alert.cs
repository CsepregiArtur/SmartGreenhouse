using System;

namespace SmartGreenhouse.Models.Entities
{
    /// <summary>
    /// Reprezintă o alertă generată de sistem
    /// </summary>
    public class Alert
    {
        public int AlertId { get; set; }
        public int? SensorId { get; set; }
        public int? ActuatorId { get; set; }
        public DateTime Timestamp { get; set; }
        public AlertLevel Level { get; set; }
        public string Message { get; set; }
        public double? Value { get; set; }
        public bool IsAcknowledged { get; set; }
        public int? AcknowledgedByUserId { get; set; }
        public DateTime? AcknowledgedAt { get; set; }
        
        // Navigation properties
        public virtual Sensor Sensor { get; set; }
        public virtual Actuator Actuator { get; set; }
        public virtual User AcknowledgedBy { get; set; }
    }

    public enum AlertLevel
    {
        Info = 1,
        Warning = 2,
        Critical = 3
    }
}
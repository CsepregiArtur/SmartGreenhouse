using System;

namespace SmartGreenhouse.Models.Entities
{
    /// <summary>
    /// Reprezintă o citire de la un senzor
    /// </summary>
    public class SensorReading
    {
        public int ReadingId { get; set; }
        public int SensorId { get; set; }
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
        public ReadingQuality Quality { get; set; }
        
        // Navigation property
        public virtual Sensor Sensor { get; set; }
    }

    public enum ReadingQuality
    {
        Normal = 1,
        Suspect = 2,
        Invalid = 3
    }
}
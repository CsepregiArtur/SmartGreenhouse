using System;

namespace SmartGreenhouse.Models.Entities
{
    /// <summary>
    /// Reprezintă o comandă dată unui actuator
    /// </summary>
    public class ActuatorCommand
    {
        public int CommandId { get; set; }
        public int ActuatorId { get; set; }
        public int IssuedByUserId { get; set; }
        public DateTime Timestamp { get; set; }
        public CommandType Command { get; set; }
        public int? DurationSeconds { get; set; }
        public bool Executed { get; set; }
        
        // Navigation properties
        public virtual Actuator Actuator { get; set; }
        public virtual User IssuedBy { get; set; }
    }

    public enum CommandType
    {
        TurnOn = 1,
        TurnOff = 2,
        SetAuto = 3,
        SetManual = 4
    }
}
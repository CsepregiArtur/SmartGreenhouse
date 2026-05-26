using System;
using System.Collections.Generic;

namespace SmartGreenhouse.Models.Entities
{
    /// <summary>
    /// Reprezintă un actuator (ventilator, pompă, etc.)
    /// </summary>
    public class Actuator
    {
        public int ActuatorId { get; set; }
        public string Name { get; set; }
        public ActuatorType Type { get; set; }
        public string Location { get; set; }
        public ActuatorStatus Status { get; set; }
        public bool IsAutoMode { get; set; }
        public DateTime InstalledDate { get; set; }
        public int CreatedByUserId { get; set; }
        
        // Navigation properties
        public virtual User CreatedBy { get; set; }
        public virtual ICollection<ActuatorCommand> Commands { get; set; }
    }

    public enum ActuatorType
    {
        Ventilation = 1,
        WaterPump = 2,
        Heater = 3,
        Light = 4,
        Window = 5
    }

    public enum ActuatorStatus
    {
        Off = 0,
        On = 1,
        Error = 2,
        Maintenance = 3
    }
}
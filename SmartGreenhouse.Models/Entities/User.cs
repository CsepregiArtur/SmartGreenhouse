using System;
using System.Collections.Generic;

namespace SmartGreenhouse.Models.Entities
{
    /// <summary>
    /// Reprezintă un utilizator al sistemului
    /// </summary>
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsActive { get; set; }
        
        // Navigation properties
        public virtual ICollection<ActuatorCommand> Commands { get; set; }
        public virtual ICollection<Alert> AcknowledgedAlerts { get; set; }
    }

    public enum UserRole
    {
        Admin = 1,
        Operator = 2,
        Viewer = 3
    }
}
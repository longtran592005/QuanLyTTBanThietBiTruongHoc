using System;

namespace DTO
{
    /// <summary>
    /// Data Transfer Object for Employee / User
    /// </summary>
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}

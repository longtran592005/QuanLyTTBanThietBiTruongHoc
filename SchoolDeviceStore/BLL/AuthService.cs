using System;
using DAL.Repositories;
using DAL.Utils;
using DTO;

namespace BLL
{
    /// <summary>
    /// Authentication and authorization related business logic.
    /// </summary>
    public class AuthService
    {
        private readonly EmployeeRepository _repo = new EmployeeRepository();

        /// <summary>
        /// Authenticate a user by username and password. Returns Employee DTO on success, null on failure.
        /// </summary>
        public Employee Authenticate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            if (!_repo.TryGetCredentials(username, out var storedHash, out var storedSalt, out var employeeId))
                return null;

            if (storedHash == null || storedSalt == null) return null;

            var ok = PasswordHelper.VerifyPassword(password, storedHash, storedSalt);
            if (!ok) return null;

            return _repo.GetByUsername(username);
        }

        public bool ChangePassword(int employeeId, string newPassword)
        {
            if (employeeId <= 0 || string.IsNullOrEmpty(newPassword)) return false;
            PasswordHelper.CreatePasswordHash(newPassword, out var hash, out var salt);
            return _repo.UpdatePassword(employeeId, hash, salt);
        }

        /// <summary>
        /// Ensure default admin exists. Call at application startup for demo environments.
        /// </summary>
        public void EnsureAdminExists(string adminUsername = "admin", string adminPassword = "admin123")
        {
            var existing = _repo.GetByUsername(adminUsername);
            if (existing != null) return;
            var emp = new Employee
            {
                Username = adminUsername,
                FullName = "System Administrator",
                Email = "admin@example.com",
                Phone = "",
                RoleId = 1,
                IsActive = true
            };
            PasswordHelper.CreatePasswordHash(adminPassword, out var hash, out var salt);
            _repo.Create(emp, hash, salt);
        }
    }
}

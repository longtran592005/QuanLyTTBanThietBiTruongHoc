using System;
using System.Collections.Generic;
using System.Data;
using DAL.Repositories;
using DAL.Utils;
using DTO;

namespace BLL
{
    /// <summary>
    /// Business logic for Employee management operations.
    /// </summary>
    public class EmployeeService
    {
        private readonly EmployeeRepository _repo = new EmployeeRepository();

        public List<Employee> GetAll()
        {
            return _repo.GetAll();
        }

        public DataTable GetAllAsDataTable()
        {
            return _repo.GetAllAsDataTable();
        }

        public Employee GetById(int employeeId)
        {
            return _repo.GetById(employeeId);
        }

        public DataTable GetRoles()
        {
            return _repo.GetRoles();
        }

        public int CreateEmployee(Employee emp, string password)
        {
            if (string.IsNullOrWhiteSpace(emp.Username))
                throw new ArgumentException("Tên đăng nhập không được để trống.");
            if (string.IsNullOrWhiteSpace(emp.FullName))
                throw new ArgumentException("Họ tên không được để trống.");
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                throw new ArgumentException("Mật khẩu phải có ít nhất 6 ký tự.");
            if (emp.RoleId <= 0)
                throw new ArgumentException("Vui lòng chọn vai trò.");

            if (_repo.UsernameExists(emp.Username))
                throw new ArgumentException($"Tên đăng nhập '{emp.Username}' đã tồn tại.");

            emp.IsActive = true;
            PasswordHelper.CreatePasswordHash(password, out var hash, out var salt);
            return _repo.Create(emp, hash, salt);
        }

        public bool UpdateEmployee(Employee emp)
        {
            if (string.IsNullOrWhiteSpace(emp.FullName))
                throw new ArgumentException("Họ tên không được để trống.");
            if (emp.RoleId <= 0)
                throw new ArgumentException("Vui lòng chọn vai trò.");

            return _repo.Update(emp);
        }

        public bool ResetPassword(int employeeId, string newPassword)
        {
            if (employeeId <= 0)
                throw new ArgumentException("Mã nhân viên không hợp lệ.");
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                throw new ArgumentException("Mật khẩu mới phải có ít nhất 6 ký tự.");

            PasswordHelper.CreatePasswordHash(newPassword, out var hash, out var salt);
            return _repo.UpdatePassword(employeeId, hash, salt);
        }

        public bool DeactivateEmployee(int employeeId)
        {
            return _repo.Deactivate(employeeId);
        }

        public bool ActivateEmployee(int employeeId)
        {
            return _repo.Activate(employeeId);
        }

        /// <summary>
        /// Get the Vietnamese display name for a role ID.
        /// </summary>
        public static string GetRoleDisplayName(int roleId)
        {
            switch (roleId)
            {
                case 1: return "Quản trị viên";
                case 2: return "Quản lý cửa hàng";
                case 3: return "Nhân viên bán hàng";
                case 4: return "Thủ kho";
                case 5: return "Kế toán";
                default: return "Nhân viên";
            }
        }

        /// <summary>
        /// Check whether a given role has permission to access a specific feature.
        /// </summary>
        public static bool HasPermission(int roleId, string feature)
        {
            switch (feature)
            {
                case "dashboard":
                    return true; // All roles can see dashboard

                case "sales":
                    return roleId == 1 || roleId == 2 || roleId == 3; // Admin, Manager, Salesperson

                case "products_read":
                    return true; // All roles can view products

                case "products_write":
                    return roleId == 1 || roleId == 2 || roleId == 4; // Admin, Manager, Warehouse

                case "categories":
                    return roleId == 1 || roleId == 2 || roleId == 4; // Admin, Manager, Warehouse(read)

                case "suppliers":
                    return roleId == 1 || roleId == 2 || roleId == 4; // Admin, Manager, Warehouse

                case "promotions":
                    return roleId == 1 || roleId == 2 || roleId == 3 || roleId == 5; // Admin, Manager, Sales(apply only), Accountant(read)

                case "promotions_write":
                    return roleId == 1 || roleId == 2; // Admin, Manager

                case "reports":
                    return roleId == 1 || roleId == 2 || roleId == 5; // Admin, Manager, Accountant

                case "employees":
                    return roleId == 1; // Admin only

                case "backup":
                    return roleId == 1; // Admin only

                default:
                    return false;
            }
        }
    }
}

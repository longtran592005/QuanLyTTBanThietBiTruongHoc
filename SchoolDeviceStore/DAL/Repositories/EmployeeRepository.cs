using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using DTO;

namespace DAL.Repositories
{
    /// <summary>
    /// Repository for employee-related DB operations. Uses DbHelper for ADO.NET access.
    /// All queries are parameterized to prevent SQL injection.
    /// </summary>
    public class EmployeeRepository
    {
        public Employee GetByUsername(string username)
        {
            string sql = @"SELECT e.EmployeeId, e.Username, e.FullName, e.Email, e.Phone, e.RoleId, r.RoleName, e.CreatedAt, e.IsActive 
                           FROM Employees e 
                           INNER JOIN Roles r ON r.RoleId = e.RoleId 
                           WHERE e.Username = @username";
            var dt = DAL.DbHelper.ExecuteQuery(sql, new SQLiteParameter("@username", username));
            if (dt.Rows.Count == 0) return null;
            return MapEmployee(dt.Rows[0]);
        }

        public Employee GetById(int employeeId)
        {
            string sql = @"SELECT e.EmployeeId, e.Username, e.FullName, e.Email, e.Phone, e.RoleId, r.RoleName, e.CreatedAt, e.IsActive 
                           FROM Employees e 
                           INNER JOIN Roles r ON r.RoleId = e.RoleId 
                           WHERE e.EmployeeId = @id";
            var dt = DAL.DbHelper.ExecuteQuery(sql, new SQLiteParameter("@id", employeeId));
            if (dt.Rows.Count == 0) return null;
            return MapEmployee(dt.Rows[0]);
        }

        public List<Employee> GetAll()
        {
            string sql = @"SELECT e.EmployeeId, e.Username, e.FullName, e.Email, e.Phone, e.RoleId, r.RoleName, e.CreatedAt, e.IsActive 
                           FROM Employees e 
                           INNER JOIN Roles r ON r.RoleId = e.RoleId 
                           ORDER BY e.EmployeeId";
            var dt = DAL.DbHelper.ExecuteQuery(sql);
            var list = new List<Employee>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapEmployee(row));
            }
            return list;
        }

        public DataTable GetAllAsDataTable()
        {
            string sql = @"SELECT e.EmployeeId AS 'Mã NV', e.Username AS 'Tên đăng nhập', e.FullName AS 'Họ tên', 
                           e.Email, e.Phone AS 'SĐT', r.RoleName AS 'Vai trò', 
                           CASE WHEN e.IsActive = 1 THEN 'Hoạt động' ELSE 'Đã khóa' END AS 'Trạng thái',
                           e.CreatedAt AS 'Ngày tạo'
                           FROM Employees e 
                           INNER JOIN Roles r ON r.RoleId = e.RoleId 
                           ORDER BY e.EmployeeId";
            return DAL.DbHelper.ExecuteQuery(sql);
        }

        public DataTable GetRoles()
        {
            return DAL.DbHelper.ExecuteQuery("SELECT RoleId, RoleName FROM Roles ORDER BY RoleId");
        }

        /// <summary>
        /// Try to get stored password hash and salt for a username.
        /// Returns true if found.
        /// </summary>
        public bool TryGetCredentials(string username, out byte[] passwordHash, out byte[] passwordSalt, out int employeeId)
        {
            passwordHash = null;
            passwordSalt = null;
            employeeId = 0;
            string sql = "SELECT EmployeeId, PasswordHash, PasswordSalt FROM Employees WHERE Username = @username AND IsActive = 1";
            var dt = DAL.DbHelper.ExecuteQuery(sql, new SQLiteParameter("@username", username));
            if (dt.Rows.Count == 0) return false;
            var r = dt.Rows[0];
            employeeId = Convert.ToInt32(r["EmployeeId"]);
            passwordHash = r["PasswordHash"] == DBNull.Value ? null : (byte[])r["PasswordHash"];
            passwordSalt = r["PasswordSalt"] == DBNull.Value ? null : (byte[])r["PasswordSalt"];
            return true;
        }

        public int Create(Employee emp, byte[] passwordHash, byte[] passwordSalt)
        {
            string sql = @"
INSERT INTO Employees (Username, PasswordHash, PasswordSalt, FullName, Email, Phone, RoleId, CreatedAt, IsActive)
VALUES (@username, @hash, @salt, @fullname, @email, @phone, @roleId, CURRENT_TIMESTAMP, @isActive);
SELECT CAST(SCOPE_IDENTITY() AS int);";
            var p = new SQLiteParameter[]
            {
                new SQLiteParameter("@username", emp.Username),
                new SQLiteParameter("@hash", passwordHash),
                new SQLiteParameter("@salt", passwordSalt),
                new SQLiteParameter("@fullname", emp.FullName),
                new SQLiteParameter("@email", (object)emp.Email ?? DBNull.Value),
                new SQLiteParameter("@phone", (object)emp.Phone ?? DBNull.Value),
                new SQLiteParameter("@roleId", emp.RoleId),
                new SQLiteParameter("@isActive", emp.IsActive)
            };
            var idObj = DAL.DbHelper.ExecuteScalar(sql, p);
            return Convert.ToInt32(idObj);
        }

        public bool Update(Employee emp)
        {
            string sql = @"UPDATE Employees SET FullName = @fullname, Email = @email, Phone = @phone, RoleId = @roleId, IsActive = @isActive 
                           WHERE EmployeeId = @id";
            var affected = DAL.DbHelper.ExecuteNonQuery(sql,
                new SQLiteParameter("@fullname", emp.FullName),
                new SQLiteParameter("@email", (object)emp.Email ?? DBNull.Value),
                new SQLiteParameter("@phone", (object)emp.Phone ?? DBNull.Value),
                new SQLiteParameter("@roleId", emp.RoleId),
                new SQLiteParameter("@isActive", emp.IsActive),
                new SQLiteParameter("@id", emp.EmployeeId));
            return affected > 0;
        }

        public bool Deactivate(int employeeId)
        {
            string sql = "UPDATE Employees SET IsActive = 0 WHERE EmployeeId = @id";
            var affected = DAL.DbHelper.ExecuteNonQuery(sql, new SQLiteParameter("@id", employeeId));
            return affected > 0;
        }

        public bool Activate(int employeeId)
        {
            string sql = "UPDATE Employees SET IsActive = 1 WHERE EmployeeId = @id";
            var affected = DAL.DbHelper.ExecuteNonQuery(sql, new SQLiteParameter("@id", employeeId));
            return affected > 0;
        }

        public bool UpdatePassword(int employeeId, byte[] hash, byte[] salt)
        {
            string sql = "UPDATE Employees SET PasswordHash = @hash, PasswordSalt = @salt WHERE EmployeeId = @id";
            var affected = DAL.DbHelper.ExecuteNonQuery(sql, new SQLiteParameter("@hash", hash), new SQLiteParameter("@salt", salt), new SQLiteParameter("@id", employeeId));
            return affected > 0;
        }

        public bool UsernameExists(string username, int? excludeEmployeeId = null)
        {
            string sql = excludeEmployeeId.HasValue
                ? "SELECT COUNT(1) FROM Employees WHERE Username = @username AND EmployeeId != @excludeId"
                : "SELECT COUNT(1) FROM Employees WHERE Username = @username";
            
            var parameters = excludeEmployeeId.HasValue
                ? new SQLiteParameter[] { new SQLiteParameter("@username", username), new SQLiteParameter("@excludeId", excludeEmployeeId.Value) }
                : new SQLiteParameter[] { new SQLiteParameter("@username", username) };

            var count = DAL.DbHelper.ExecuteScalar(sql, parameters);
            return Convert.ToInt64(count) > 0;
        }

        private static Employee MapEmployee(DataRow r)
        {
            return new Employee
            {
                EmployeeId = Convert.ToInt32(r["EmployeeId"]),
                Username = r["Username"].ToString(),
                FullName = r["FullName"].ToString(),
                Email = r["Email"] == DBNull.Value ? null : r["Email"].ToString(),
                Phone = r["Phone"] == DBNull.Value ? null : r["Phone"].ToString(),
                RoleId = Convert.ToInt32(r["RoleId"]),
                RoleName = r["RoleName"] == DBNull.Value ? null : r["RoleName"].ToString(),
                CreatedAt = Convert.ToDateTime(r["CreatedAt"]),
                IsActive = Convert.ToBoolean(r["IsActive"])
            };
        }
    }
}

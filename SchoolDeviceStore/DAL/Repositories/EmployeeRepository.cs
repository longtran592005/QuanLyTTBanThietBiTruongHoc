using System;
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
            string sql = "SELECT EmployeeId, Username, FullName, Email, Phone, RoleId, CreatedAt, IsActive FROM Employees WHERE Username = @username";
            var dt = DAL.DbHelper.ExecuteQuery(sql, new SQLiteParameter("@username", username));
            if (dt.Rows.Count == 0) return null;
            var r = dt.Rows[0];
            return new Employee
            {
                EmployeeId = Convert.ToInt32(r["EmployeeId"]),
                Username = r["Username"].ToString(),
                FullName = r["FullName"].ToString(),
                Email = r["Email"] == DBNull.Value ? null : r["Email"].ToString(),
                Phone = r["Phone"] == DBNull.Value ? null : r["Phone"].ToString(),
                RoleId = Convert.ToInt32(r["RoleId"]),
                CreatedAt = Convert.ToDateTime(r["CreatedAt"]),
                IsActive = Convert.ToBoolean(r["IsActive"]) 
            };
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
            string sql = "SELECT EmployeeId, PasswordHash, PasswordSalt FROM Employees WHERE Username = @username";
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
SELECT last_insert_rowid();";
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

        public bool UpdatePassword(int employeeId, byte[] hash, byte[] salt)
        {
            string sql = "UPDATE Employees SET PasswordHash = @hash, PasswordSalt = @salt WHERE EmployeeId = @id";
            var affected = DAL.DbHelper.ExecuteNonQuery(sql, new SQLiteParameter("@hash", hash), new SQLiteParameter("@salt", salt), new SQLiteParameter("@id", employeeId));
            return affected > 0;
        }
    }
}

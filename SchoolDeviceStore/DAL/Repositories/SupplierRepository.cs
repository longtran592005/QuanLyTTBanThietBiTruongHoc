using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using DTO;

namespace DAL.Repositories
{
    public class SupplierRepository
    {
        public List<Supplier> GetAll()
        {
            var list = new List<Supplier>();
            string sql = "SELECT SupplierId, SupplierName, ContactName, Phone, Email, Address FROM Suppliers";
            var dt = DAL.DbHelper.ExecuteQuery(sql);
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new Supplier
                {
                    SupplierId = Convert.ToInt32(r["SupplierId"]),
                    SupplierName = r["SupplierName"].ToString(),
                    ContactName = r["ContactName"] == DBNull.Value ? null : r["ContactName"].ToString(),
                    Phone = r["Phone"] == DBNull.Value ? null : r["Phone"].ToString(),
                    Email = r["Email"] == DBNull.Value ? null : r["Email"].ToString(),
                    Address = r["Address"] == DBNull.Value ? null : r["Address"].ToString()
                });
            }
            return list;
        }

        public Supplier GetById(int id)
        {
            string sql = "SELECT SupplierId, SupplierName, ContactName, Phone, Email, Address FROM Suppliers WHERE SupplierId = @id";
            var dt = DAL.DbHelper.ExecuteQuery(sql, new SQLiteParameter("@id", id));
            if (dt.Rows.Count == 0) return null;
            var r = dt.Rows[0];
            return new Supplier
            {
                SupplierId = Convert.ToInt32(r["SupplierId"]),
                SupplierName = r["SupplierName"].ToString(),
                ContactName = r["ContactName"] == DBNull.Value ? null : r["ContactName"].ToString(),
                Phone = r["Phone"] == DBNull.Value ? null : r["Phone"].ToString(),
                Email = r["Email"] == DBNull.Value ? null : r["Email"].ToString(),
                Address = r["Address"] == DBNull.Value ? null : r["Address"].ToString()
            };
        }

        public int Create(Supplier supplier)
        {
            string sql = @"INSERT INTO Suppliers (SupplierName, ContactName, Phone, Email, Address)
VALUES (@name, @contact, @phone, @email, @address);
SELECT last_insert_rowid();";
            var id = DAL.DbHelper.ExecuteScalar(sql,
                new SQLiteParameter("@name", supplier.SupplierName),
                new SQLiteParameter("@contact", (object)supplier.ContactName ?? DBNull.Value),
                new SQLiteParameter("@phone", (object)supplier.Phone ?? DBNull.Value),
                new SQLiteParameter("@email", (object)supplier.Email ?? DBNull.Value),
                new SQLiteParameter("@address", (object)supplier.Address ?? DBNull.Value));
            return Convert.ToInt32(id);
        }

        public bool Update(Supplier supplier)
        {
            string sql = @"UPDATE Suppliers SET SupplierName = @name, ContactName = @contact, Phone = @phone, Email = @email, Address = @address
WHERE SupplierId = @id";
            var affected = DAL.DbHelper.ExecuteNonQuery(sql,
                new SQLiteParameter("@name", supplier.SupplierName),
                new SQLiteParameter("@contact", (object)supplier.ContactName ?? DBNull.Value),
                new SQLiteParameter("@phone", (object)supplier.Phone ?? DBNull.Value),
                new SQLiteParameter("@email", (object)supplier.Email ?? DBNull.Value),
                new SQLiteParameter("@address", (object)supplier.Address ?? DBNull.Value),
                new SQLiteParameter("@id", supplier.SupplierId));
            return affected > 0;
        }

        public bool Delete(int id)
        {
            string sql = "DELETE FROM Suppliers WHERE SupplierId = @id";
            var affected = DAL.DbHelper.ExecuteNonQuery(sql, new SQLiteParameter("@id", id));
            return affected > 0;
        }
    }
}

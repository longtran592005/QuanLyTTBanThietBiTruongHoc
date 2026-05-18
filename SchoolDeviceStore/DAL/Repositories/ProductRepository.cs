using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using DTO;

namespace DAL.Repositories
{
    /// <summary>
    /// Repository for product CRUD operations using SQLite and parameterized queries.
    /// </summary>
    public class ProductRepository
    {
        public List<Product> GetAll()
        {
            var list = new List<Product>();
            string sql = "SELECT ProductId, ProductCode, ProductName, CategoryId, ManufacturerId, SupplierId, Quantity, UnitPrice, PurchasePrice, ImagePath, Description, Status FROM Products";
            var dt = DAL.DbHelper.ExecuteQuery(sql);
            foreach (DataRow r in dt.Rows)
            {
                list.Add(Map(r));
            }
            return list;
        }

        public Product GetById(int id)
        {
            string sql = "SELECT ProductId, ProductCode, ProductName, CategoryId, ManufacturerId, SupplierId, Quantity, UnitPrice, PurchasePrice, ImagePath, Description, Status FROM Products WHERE ProductId = @id";
            var dt = DAL.DbHelper.ExecuteQuery(sql, new SQLiteParameter("@id", id));
            if (dt.Rows.Count == 0) return null;
            return Map(dt.Rows[0]);
        }

        public List<Product> Search(string keyword)
        {
            var list = new List<Product>();
            string sql = "SELECT ProductId, ProductCode, ProductName, CategoryId, ManufacturerId, SupplierId, Quantity, UnitPrice, PurchasePrice, ImagePath, Description, Status FROM Products WHERE ProductName LIKE @kw OR ProductCode LIKE @kw";
            var dt = DAL.DbHelper.ExecuteQuery(sql, new SQLiteParameter("@kw", "%" + keyword + "%"));
            foreach (DataRow r in dt.Rows)
                list.Add(Map(r));
            return list;
        }

        public int Create(Product p)
        {
            string sql = @"INSERT INTO Products (ProductCode, ProductName, CategoryId, ManufacturerId, SupplierId, Quantity, UnitPrice, PurchasePrice, ImagePath, Description, Status)
VALUES (@code,@name,@cat,@man,@sup,@qty,@unit,@purch,@img,@desc,@status); SELECT last_insert_rowid();";
            var parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@code", p.ProductCode),
                new SQLiteParameter("@name", p.ProductName),
                new SQLiteParameter("@cat", (object)p.CategoryId ?? DBNull.Value),
                new SQLiteParameter("@man", (object)p.ManufacturerId ?? DBNull.Value),
                new SQLiteParameter("@sup", (object)p.SupplierId ?? DBNull.Value),
                new SQLiteParameter("@qty", p.Quantity),
                new SQLiteParameter("@unit", p.UnitPrice),
                new SQLiteParameter("@purch", p.PurchasePrice),
                new SQLiteParameter("@img", (object)p.ImagePath ?? DBNull.Value),
                new SQLiteParameter("@desc", (object)p.Description ?? DBNull.Value),
                new SQLiteParameter("@status", (object)p.Status ?? DBNull.Value)
            };
            var obj = DAL.DbHelper.ExecuteScalar(sql, parameters);
            return Convert.ToInt32(obj);
        }

        public bool Update(Product p)
        {
            string sql = @"UPDATE Products SET ProductCode=@code, ProductName=@name, CategoryId=@cat, ManufacturerId=@man, SupplierId=@sup, Quantity=@qty, UnitPrice=@unit, PurchasePrice=@purch, ImagePath=@img, Description=@desc, Status=@status WHERE ProductId=@id";
            var parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@code", p.ProductCode),
                new SQLiteParameter("@name", p.ProductName),
                new SQLiteParameter("@cat", (object)p.CategoryId ?? DBNull.Value),
                new SQLiteParameter("@man", (object)p.ManufacturerId ?? DBNull.Value),
                new SQLiteParameter("@sup", (object)p.SupplierId ?? DBNull.Value),
                new SQLiteParameter("@qty", p.Quantity),
                new SQLiteParameter("@unit", p.UnitPrice),
                new SQLiteParameter("@purch", p.PurchasePrice),
                new SQLiteParameter("@img", (object)p.ImagePath ?? DBNull.Value),
                new SQLiteParameter("@desc", (object)p.Description ?? DBNull.Value),
                new SQLiteParameter("@status", (object)p.Status ?? DBNull.Value),
                new SQLiteParameter("@id", p.ProductId)
            };
            var affected = DAL.DbHelper.ExecuteNonQuery(sql, parameters);
            return affected > 0;
        }

        public bool Delete(int productId)
        {
            string sql = "DELETE FROM Products WHERE ProductId = @id";
            var affected = DAL.DbHelper.ExecuteNonQuery(sql, new SQLiteParameter("@id", productId));
            return affected > 0;
        }

        public bool ExistsByCode(string productCode)
        {
            const string sql = "SELECT COUNT(1) FROM Products WHERE ProductCode = @code";
            var result = DAL.DbHelper.ExecuteScalar(sql, new SQLiteParameter("@code", productCode));
            return Convert.ToInt32(result) > 0;
        }

        public bool ExistsByCodeExceptId(string productCode, int productId)
        {
            const string sql = "SELECT COUNT(1) FROM Products WHERE ProductCode = @code AND ProductId <> @id";
            var result = DAL.DbHelper.ExecuteScalar(sql, new SQLiteParameter("@code", productCode), new SQLiteParameter("@id", productId));
            return Convert.ToInt32(result) > 0;
        }

        public bool UpdateQuantity(int productId, int newQuantity)
        {
            string sql = "UPDATE Products SET Quantity = @q WHERE ProductId = @id";
            var affected = DAL.DbHelper.ExecuteNonQuery(sql, new SQLiteParameter("@q", newQuantity), new SQLiteParameter("@id", productId));
            return affected > 0;
        }

        private Product Map(DataRow r)
        {
            return new Product
            {
                ProductId = Convert.ToInt32(r["ProductId"]),
                ProductCode = r["ProductCode"].ToString(),
                ProductName = r["ProductName"].ToString(),
                CategoryId = r["CategoryId"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["CategoryId"]),
                ManufacturerId = r["ManufacturerId"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["ManufacturerId"]),
                SupplierId = r["SupplierId"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["SupplierId"]),
                Quantity = r["Quantity"] == DBNull.Value ? 0 : Convert.ToInt32(r["Quantity"]),
                UnitPrice = r["UnitPrice"] == DBNull.Value ? 0m : Convert.ToDecimal(r["UnitPrice"]),
                PurchasePrice = r["PurchasePrice"] == DBNull.Value ? 0m : Convert.ToDecimal(r["PurchasePrice"]),
                ImagePath = r["ImagePath"] == DBNull.Value ? null : r["ImagePath"].ToString(),
                Description = r["Description"] == DBNull.Value ? null : r["Description"].ToString(),
                Status = r["Status"] == DBNull.Value ? null : r["Status"].ToString()
            };
        }
    }
}

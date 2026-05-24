using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using DTO;

namespace DAL.Repositories
{
    public class CategoryRepository
    {
        public List<Category> GetAll()
        {
            var list = new List<Category>();
            string sql = "SELECT CategoryId, CategoryName, Description FROM Categories";
            var dt = DAL.DbHelper.ExecuteQuery(sql);
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new Category
                {
                    CategoryId = Convert.ToInt32(r["CategoryId"]),
                    CategoryName = r["CategoryName"].ToString(),
                    Description = r["Description"] == DBNull.Value ? null : r["Description"].ToString()
                });
            }
            return list;
        }

        public Category GetById(int id)
        {
            string sql = "SELECT CategoryId, CategoryName, Description FROM Categories WHERE CategoryId = @id";
            var dt = DAL.DbHelper.ExecuteQuery(sql, new SQLiteParameter("@id", id));
            if (dt.Rows.Count == 0) return null;
            var r = dt.Rows[0];
            return new Category
            {
                CategoryId = Convert.ToInt32(r["CategoryId"]),
                CategoryName = r["CategoryName"].ToString(),
                Description = r["Description"] == DBNull.Value ? null : r["Description"].ToString()
            };
        }

        public int Create(Category category)
        {
            string sql = @"INSERT INTO Categories (CategoryName, Description)
VALUES (@name, @desc);
SELECT CAST(SCOPE_IDENTITY() AS int);";
            var id = DAL.DbHelper.ExecuteScalar(sql,
                new SQLiteParameter("@name", category.CategoryName),
                new SQLiteParameter("@desc", (object)category.Description ?? DBNull.Value));
            return Convert.ToInt32(id);
        }

        public bool Update(Category category)
        {
            string sql = "UPDATE Categories SET CategoryName = @name, Description = @desc WHERE CategoryId = @id";
            var affected = DAL.DbHelper.ExecuteNonQuery(sql,
                new SQLiteParameter("@name", category.CategoryName),
                new SQLiteParameter("@desc", (object)category.Description ?? DBNull.Value),
                new SQLiteParameter("@id", category.CategoryId));
            return affected > 0;
        }

        public bool Delete(int id)
        {
            string sql = "DELETE FROM Categories WHERE CategoryId = @id";
            var affected = DAL.DbHelper.ExecuteNonQuery(sql, new SQLiteParameter("@id", id));
            return affected > 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Data.SqlClient;
using DTO;

namespace DAL.Repositories
{
    public class InventoryLogRepository
    {
        public int RecordChange(int productId, int change, string reason, int? changedBy)
        {
            const string sql = @"INSERT INTO InventoryLogs (ProductId, Change, Reason, ChangedBy, ChangedAt)
VALUES (@productId, @change, @reason, @changedBy, CURRENT_TIMESTAMP); SELECT CAST(SCOPE_IDENTITY() AS int);";

            var id = DAL.DbHelper.ExecuteScalar(sql,
                new SQLiteParameter("@productId", productId),
                new SQLiteParameter("@change", change),
                new SQLiteParameter("@reason", (object)reason ?? DBNull.Value),
                new SQLiteParameter("@changedBy", (object)changedBy ?? DBNull.Value));

            return Convert.ToInt32(id);
        }

        public int RecordChange(SQLiteConnection conn, SQLiteTransaction tx, int productId, int change, string reason, int? changedBy)
        {
            const string sql = @"INSERT INTO InventoryLogs (ProductId, Change, Reason, ChangedBy, ChangedAt)
VALUES (@productId, @change, @reason, @changedBy, CURRENT_TIMESTAMP); SELECT CAST(SCOPE_IDENTITY() AS int);";

            using (var cmd = new SQLiteCommand(sql, conn, tx))
            {
                cmd.Parameters.AddWithValue("@productId", productId);
                cmd.Parameters.AddWithValue("@change", change);
                cmd.Parameters.AddWithValue("@reason", (object)reason ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@changedBy", (object)changedBy ?? DBNull.Value);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public int RecordChange(SqlConnection conn, SqlTransaction tx, int productId, int change, string reason, int? changedBy)
        {
            const string sql = @"INSERT INTO InventoryLogs (ProductId, Change, Reason, ChangedBy, ChangedAt)
VALUES (@productId, @change, @reason, @changedBy, CURRENT_TIMESTAMP); SELECT CAST(SCOPE_IDENTITY() AS int);";

            using (var cmd = new SqlCommand(sql, conn, tx))
            {
                cmd.Parameters.AddWithValue("@productId", productId);
                cmd.Parameters.AddWithValue("@change", change);
                cmd.Parameters.AddWithValue("@reason", (object)reason ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@changedBy", (object)changedBy ?? DBNull.Value);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public List<InventoryLog> GetByProductId(int productId)
        {
            const string sql = @"SELECT InventoryLogId, ProductId, Change, Reason, ChangedBy, ChangedAt
FROM InventoryLogs WHERE ProductId = @productId ORDER BY ChangedAt DESC, InventoryLogId DESC";
            var dt = DAL.DbHelper.ExecuteQuery(sql, new SQLiteParameter("@productId", productId));
            var list = new List<InventoryLog>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new InventoryLog
                {
                    InventoryLogId = Convert.ToInt32(row["InventoryLogId"]),
                    ProductId = Convert.ToInt32(row["ProductId"]),
                    Change = Convert.ToInt32(row["Change"]),
                    Reason = row["Reason"] == DBNull.Value ? null : row["Reason"].ToString(),
                    ChangedBy = row["ChangedBy"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["ChangedBy"]),
                    ChangedAt = DateTime.Parse(row["ChangedAt"].ToString())
                });
            }

            return list;
        }

        public List<InventoryLog> GetByFilter(int? productId, DateTime? from, DateTime? to, int offset, int limit)
        {
            const string sql = @"SELECT InventoryLogId, ProductId, Change, Reason, ChangedBy, ChangedAt
FROM InventoryLogs
WHERE (@productId IS NULL OR ProductId = @productId)
  AND (@from IS NULL OR ChangedAt >= @from)
  AND (@to IS NULL OR ChangedAt <= @to)
ORDER BY ChangedAt DESC, InventoryLogId DESC
OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY";

            var parameters = new List<SQLiteParameter>
            {
                new SQLiteParameter("@productId", (object)productId ?? DBNull.Value),
                new SQLiteParameter("@from", (object)from?.ToString("yyyy-MM-dd HH:mm:ss") ?? DBNull.Value),
                new SQLiteParameter("@to", (object)to?.ToString("yyyy-MM-dd HH:mm:ss") ?? DBNull.Value),
                new SQLiteParameter("@limit", limit),
                new SQLiteParameter("@offset", offset)
            };

            var dt = DAL.DbHelper.ExecuteQuery(sql, parameters.ToArray());
            var list = new List<InventoryLog>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new InventoryLog
                {
                    InventoryLogId = Convert.ToInt32(row["InventoryLogId"]),
                    ProductId = Convert.ToInt32(row["ProductId"]),
                    Change = Convert.ToInt32(row["Change"]),
                    Reason = row["Reason"] == DBNull.Value ? null : row["Reason"].ToString(),
                    ChangedBy = row["ChangedBy"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["ChangedBy"]),
                    ChangedAt = DateTime.Parse(row["ChangedAt"].ToString())
                });
            }

            return list;
        }

        public int GetCountByFilter(int? productId, DateTime? from, DateTime? to)
        {
            const string sql = @"SELECT COUNT(*) FROM InventoryLogs
WHERE (@productId IS NULL OR ProductId = @productId)
  AND (@from IS NULL OR ChangedAt >= @from)
  AND (@to IS NULL OR ChangedAt <= @to)";

            var parameters = new List<SQLiteParameter>
            {
                new SQLiteParameter("@productId", (object)productId ?? DBNull.Value),
                new SQLiteParameter("@from", (object)from?.ToString("yyyy-MM-dd HH:mm:ss") ?? DBNull.Value),
                new SQLiteParameter("@to", (object)to?.ToString("yyyy-MM-dd HH:mm:ss") ?? DBNull.Value)
            };

            var result = DAL.DbHelper.ExecuteScalar(sql, parameters.ToArray());
            return Convert.ToInt32(result);
        }
    }
}
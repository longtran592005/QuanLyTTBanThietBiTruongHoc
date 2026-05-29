using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using DTO;

namespace DAL.Repositories
{
    /// <summary>
    /// Repository for Promotion-related DB operations.
    /// </summary>
    public class PromotionRepository
    {
        public List<Promotion> GetAll()
        {
            string sql = @"SELECT * FROM Promotions ORDER BY CreatedAt DESC";
            var dt = DAL.DbHelper.ExecuteQuery(sql);
            var list = new List<Promotion>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapPromotion(row));
            }
            return list;
        }

        public DataTable GetAllAsDataTable()
        {
            string sql = @"SELECT PromotionId AS 'Mã KM', PromotionCode AS 'Mã code', PromotionName AS 'Tên chương trình',
                           CASE DiscountType WHEN 'Percentage' THEN 'Phần trăm (%)' ELSE 'Số tiền cố định' END AS 'Loại giảm giá',
                           DiscountValue AS 'Giá trị',
                           MinOrderAmount AS 'Đơn tối thiểu',
                           StartDate AS 'Ngày bắt đầu', EndDate AS 'Ngày kết thúc',
                           CASE WHEN UsageLimit IS NULL THEN 'Không giới hạn' ELSE (CAST(UsageCount AS TEXT) || '/' || CAST(UsageLimit AS TEXT)) END AS 'Sử dụng',
                           CASE 
                               WHEN IsActive = 0 THEN 'Ngừng hoạt động'
                               WHEN date('now') < date(StartDate) THEN 'Sắp diễn ra'
                               WHEN date('now') > date(EndDate) THEN 'Đã hết hạn'
                               WHEN UsageLimit IS NOT NULL AND UsageCount >= UsageLimit THEN 'Đã hết lượt'
                               ELSE 'Đang hoạt động'
                           END AS 'Trạng thái'
                           FROM Promotions ORDER BY CreatedAt DESC";
            return DAL.DbHelper.ExecuteQuery(sql);
        }

        public Promotion GetById(int promotionId)
        {
            string sql = "SELECT * FROM Promotions WHERE PromotionId = @id";
            var dt = DAL.DbHelper.ExecuteQuery(sql, new SQLiteParameter("@id", promotionId));
            if (dt.Rows.Count == 0) return null;
            return MapPromotion(dt.Rows[0]);
        }

        public Promotion GetByCode(string code)
        {
            string sql = "SELECT * FROM Promotions WHERE PromotionCode = @code";
            var dt = DAL.DbHelper.ExecuteQuery(sql, new SQLiteParameter("@code", code));
            if (dt.Rows.Count == 0) return null;
            return MapPromotion(dt.Rows[0]);
        }

        public List<Promotion> GetActivePromotions()
        {
            string sql = @"SELECT * FROM Promotions 
                           WHERE IsActive = 1 
                           AND date('now') >= date(StartDate) 
                           AND date('now') <= date(EndDate)
                           AND (UsageLimit IS NULL OR UsageCount < UsageLimit)
                           ORDER BY PromotionName";
            var dt = DAL.DbHelper.ExecuteQuery(sql);
            var list = new List<Promotion>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapPromotion(row));
            }
            return list;
        }

        public int Create(Promotion promo)
        {
            string sql = @"INSERT INTO Promotions (PromotionCode, PromotionName, Description, DiscountType, DiscountValue, 
                           MinOrderAmount, MaxDiscountAmount, StartDate, EndDate, UsageLimit, UsageCount, IsActive, 
                           AppliesTo, ApplyTargetId, CreatedBy, CreatedAt)
                           VALUES (@code, @name, @desc, @discountType, @discountValue, @minOrder, @maxDiscount,
                           @startDate, @endDate, @usageLimit, 0, @isActive, @appliesTo, @applyTargetId, @createdBy, CURRENT_TIMESTAMP);
                           SELECT CAST(SCOPE_IDENTITY() AS int);";
            var idObj = DAL.DbHelper.ExecuteScalar(sql,
                new SQLiteParameter("@code", promo.PromotionCode),
                new SQLiteParameter("@name", promo.PromotionName),
                new SQLiteParameter("@desc", (object)promo.Description ?? DBNull.Value),
                new SQLiteParameter("@discountType", promo.DiscountType),
                new SQLiteParameter("@discountValue", promo.DiscountValue),
                new SQLiteParameter("@minOrder", promo.MinOrderAmount),
                new SQLiteParameter("@maxDiscount", (object)promo.MaxDiscountAmount ?? DBNull.Value),
                new SQLiteParameter("@startDate", promo.StartDate.ToString("yyyy-MM-dd")),
                new SQLiteParameter("@endDate", promo.EndDate.ToString("yyyy-MM-dd")),
                new SQLiteParameter("@usageLimit", (object)promo.UsageLimit ?? DBNull.Value),
                new SQLiteParameter("@isActive", promo.IsActive ? 1 : 0),
                new SQLiteParameter("@appliesTo", promo.AppliesTo ?? "All"),
                new SQLiteParameter("@applyTargetId", (object)promo.ApplyTargetId ?? DBNull.Value),
                new SQLiteParameter("@createdBy", (object)promo.CreatedBy ?? DBNull.Value));
            return Convert.ToInt32(idObj);
        }

        public bool Update(Promotion promo)
        {
            string sql = @"UPDATE Promotions SET PromotionCode = @code, PromotionName = @name, Description = @desc,
                           DiscountType = @discountType, DiscountValue = @discountValue, MinOrderAmount = @minOrder,
                           MaxDiscountAmount = @maxDiscount, StartDate = @startDate, EndDate = @endDate,
                           UsageLimit = @usageLimit, IsActive = @isActive, AppliesTo = @appliesTo, ApplyTargetId = @applyTargetId
                           WHERE PromotionId = @id";
            var affected = DAL.DbHelper.ExecuteNonQuery(sql,
                new SQLiteParameter("@code", promo.PromotionCode),
                new SQLiteParameter("@name", promo.PromotionName),
                new SQLiteParameter("@desc", (object)promo.Description ?? DBNull.Value),
                new SQLiteParameter("@discountType", promo.DiscountType),
                new SQLiteParameter("@discountValue", promo.DiscountValue),
                new SQLiteParameter("@minOrder", promo.MinOrderAmount),
                new SQLiteParameter("@maxDiscount", (object)promo.MaxDiscountAmount ?? DBNull.Value),
                new SQLiteParameter("@startDate", promo.StartDate.ToString("yyyy-MM-dd")),
                new SQLiteParameter("@endDate", promo.EndDate.ToString("yyyy-MM-dd")),
                new SQLiteParameter("@usageLimit", (object)promo.UsageLimit ?? DBNull.Value),
                new SQLiteParameter("@isActive", promo.IsActive ? 1 : 0),
                new SQLiteParameter("@appliesTo", promo.AppliesTo ?? "All"),
                new SQLiteParameter("@applyTargetId", (object)promo.ApplyTargetId ?? DBNull.Value),
                new SQLiteParameter("@id", promo.PromotionId));
            return affected > 0;
        }

        public bool IncrementUsage(int promotionId)
        {
            string sql = "UPDATE Promotions SET UsageCount = UsageCount + 1 WHERE PromotionId = @id";
            var affected = DAL.DbHelper.ExecuteNonQuery(sql, new SQLiteParameter("@id", promotionId));
            return affected > 0;
        }

        public bool Delete(int promotionId)
        {
            string sql = "DELETE FROM Promotions WHERE PromotionId = @id";
            var affected = DAL.DbHelper.ExecuteNonQuery(sql, new SQLiteParameter("@id", promotionId));
            return affected > 0;
        }

        public bool CodeExists(string code, int? excludeId = null)
        {
            string sql = excludeId.HasValue
                ? "SELECT COUNT(1) FROM Promotions WHERE PromotionCode = @code AND PromotionId != @excludeId"
                : "SELECT COUNT(1) FROM Promotions WHERE PromotionCode = @code";
            var parameters = excludeId.HasValue
                ? new SQLiteParameter[] { new SQLiteParameter("@code", code), new SQLiteParameter("@excludeId", excludeId.Value) }
                : new SQLiteParameter[] { new SQLiteParameter("@code", code) };
            var count = DAL.DbHelper.ExecuteScalar(sql, parameters);
            return Convert.ToInt64(count) > 0;
        }

        private static Promotion MapPromotion(DataRow r)
        {
            return new Promotion
            {
                PromotionId = Convert.ToInt32(r["PromotionId"]),
                PromotionCode = r["PromotionCode"].ToString(),
                PromotionName = r["PromotionName"].ToString(),
                Description = r["Description"] == DBNull.Value ? null : r["Description"].ToString(),
                DiscountType = r["DiscountType"].ToString(),
                DiscountValue = Convert.ToDecimal(r["DiscountValue"]),
                MinOrderAmount = r["MinOrderAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(r["MinOrderAmount"]),
                MaxDiscountAmount = r["MaxDiscountAmount"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["MaxDiscountAmount"]),
                StartDate = DateTime.Parse(r["StartDate"].ToString()),
                EndDate = DateTime.Parse(r["EndDate"].ToString()),
                UsageLimit = r["UsageLimit"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["UsageLimit"]),
                UsageCount = r["UsageCount"] == DBNull.Value ? 0 : Convert.ToInt32(r["UsageCount"]),
                IsActive = Convert.ToBoolean(r["IsActive"]),
                AppliesTo = r["AppliesTo"] == DBNull.Value ? "All" : r["AppliesTo"].ToString(),
                ApplyTargetId = r["ApplyTargetId"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["ApplyTargetId"]),
                CreatedBy = r["CreatedBy"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["CreatedBy"]),
                CreatedAt = DateTime.Parse(r["CreatedAt"].ToString())
            };
        }
    }
}

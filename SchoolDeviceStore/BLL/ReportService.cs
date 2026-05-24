using System;
using System.Data;
using System.Data.SQLite;

namespace BLL
{
    /// <summary>
    /// Enhanced ReportService with filtering, comparison, category breakdown, alerts, and forecasting.
    /// </summary>
    public class ReportService
    {
        public DataTable GetRevenueByDay(DateTime fromDate, DateTime toDate)
        {
            string sql = @"SELECT CONVERT(date, OrderDate) AS ReportDate,
SUM(TotalAmount) AS Revenue,
SUM(SubTotal) AS SubTotal,
SUM(Discount) AS Discount,
SUM(VAT) AS VAT,
COUNT(1) AS OrderCount
FROM SalesOrders
WHERE CONVERT(date, OrderDate) >= CONVERT(date, @fromDate) AND CONVERT(date, OrderDate) < DATEADD(day, 1, CONVERT(date, @toDate))
GROUP BY CONVERT(date, OrderDate)
ORDER BY ReportDate";
            return DAL.DbHelper.ExecuteQuery(sql,
                new SQLiteParameter("@fromDate", fromDate.Date),
                new SQLiteParameter("@toDate", toDate.Date));
        }

        /// <summary>
        /// Get revenue by day filtered by category, supplier, and/or employee.
        /// </summary>
        public DataTable GetRevenueByDayFiltered(DateTime fromDate, DateTime toDate, 
            int? categoryId = null, int? supplierId = null, int? employeeId = null)
        {
            string sql = @"SELECT CONVERT(date, s.OrderDate) AS ReportDate,
SUM(s.TotalAmount) AS Revenue,
SUM(s.SubTotal) AS SubTotal,
SUM(s.Discount) AS Discount,
SUM(s.VAT) AS VAT,
COUNT(DISTINCT s.SalesOrderId) AS OrderCount
FROM SalesOrders s";

            // Join details only if category or supplier filter is active
            if (categoryId.HasValue || supplierId.HasValue)
            {
                sql += @"
INNER JOIN SalesOrderDetails d ON d.SalesOrderId = s.SalesOrderId
INNER JOIN Products p ON p.ProductId = d.ProductId";
            }

            sql += @"
WHERE CONVERT(date, s.OrderDate) >= CONVERT(date, @fromDate) AND CONVERT(date, s.OrderDate) < DATEADD(day, 1, CONVERT(date, @toDate))";

            if (categoryId.HasValue)
                sql += " AND p.CategoryId = @categoryId";
            if (supplierId.HasValue)
                sql += " AND p.SupplierId = @supplierId";
            if (employeeId.HasValue)
                sql += " AND s.CreatedBy = @employeeId";

            sql += " GROUP BY CONVERT(date, s.OrderDate) ORDER BY ReportDate";

            var parameters = new System.Collections.Generic.List<SQLiteParameter>
            {
                new SQLiteParameter("@fromDate", fromDate.Date),
                new SQLiteParameter("@toDate", toDate.Date)
            };
            if (categoryId.HasValue) parameters.Add(new SQLiteParameter("@categoryId", categoryId.Value));
            if (supplierId.HasValue) parameters.Add(new SQLiteParameter("@supplierId", supplierId.Value));
            if (employeeId.HasValue) parameters.Add(new SQLiteParameter("@employeeId", employeeId.Value));

            return DAL.DbHelper.ExecuteQuery(sql, parameters.ToArray());
        }

        public DataTable GetTopProducts(DateTime fromDate, DateTime toDate)
        {
            string sql = @"SELECT TOP 10 p.ProductCode, p.ProductName, SUM(d.Quantity) AS SoldQuantity,
SUM(d.Quantity * d.UnitPrice) AS SalesAmount
FROM SalesOrderDetails d
INNER JOIN SalesOrders s ON s.SalesOrderId = d.SalesOrderId
INNER JOIN Products p ON p.ProductId = d.ProductId
WHERE CONVERT(date, s.OrderDate) >= CONVERT(date, @fromDate) AND CONVERT(date, s.OrderDate) < DATEADD(day, 1, CONVERT(date, @toDate))
GROUP BY p.ProductCode, p.ProductName
ORDER BY SoldQuantity DESC";
            return DAL.DbHelper.ExecuteQuery(sql,
                new SQLiteParameter("@fromDate", fromDate.Date),
                new SQLiteParameter("@toDate", toDate.Date));
        }

        /// <summary>
        /// Get revenue breakdown by product category for Donut chart.
        /// </summary>
        public DataTable GetRevenueByCategory(DateTime fromDate, DateTime toDate)
        {
            string sql = @"SELECT c.CategoryName, SUM(d.Quantity * d.UnitPrice) AS Revenue
FROM SalesOrderDetails d
INNER JOIN SalesOrders s ON s.SalesOrderId = d.SalesOrderId
INNER JOIN Products p ON p.ProductId = d.ProductId
LEFT JOIN Categories c ON c.CategoryId = p.CategoryId
WHERE CONVERT(date, s.OrderDate) >= CONVERT(date, @fromDate) AND CONVERT(date, s.OrderDate) < DATEADD(day, 1, CONVERT(date, @toDate))
GROUP BY c.CategoryName
ORDER BY Revenue DESC";
            return DAL.DbHelper.ExecuteQuery(sql,
                new SQLiteParameter("@fromDate", fromDate.Date),
                new SQLiteParameter("@toDate", toDate.Date));
        }

        /// <summary>
        /// Get summary KPIs for a date range: total revenue, order count, avg order value
        /// </summary>
        public ReportKpiData GetKpis(DateTime fromDate, DateTime toDate)
        {
            string sql = @"SELECT ISNULL(SUM(TotalAmount), 0) AS TotalRevenue, 
                           COUNT(1) AS OrderCount,
                           ISNULL(AVG(TotalAmount), 0) AS AvgOrderValue
                           FROM SalesOrders 
                           WHERE CONVERT(date, OrderDate) >= CONVERT(date, @fromDate) AND CONVERT(date, OrderDate) < DATEADD(day, 1, CONVERT(date, @toDate))";
            var dt = DAL.DbHelper.ExecuteQuery(sql,
                new SQLiteParameter("@fromDate", fromDate.Date),
                new SQLiteParameter("@toDate", toDate.Date));
            if (dt.Rows.Count == 0)
                return new ReportKpiData();

            var row = dt.Rows[0];
            return new ReportKpiData
            {
                TotalRevenue = Convert.ToDecimal(row["TotalRevenue"]),
                OrderCount = Convert.ToInt32(row["OrderCount"]),
                AvgOrderValue = Convert.ToDecimal(row["AvgOrderValue"])
            };
        }

        /// <summary>
        /// Get KPIs for the previous equivalent period for growth comparison.
        /// E.g., if current range is 30 days, previous range is the 30 days before that.
        /// </summary>
        public ReportKpiData GetPreviousPeriodKpis(DateTime fromDate, DateTime toDate)
        {
            var span = toDate - fromDate;
            var prevTo = fromDate;
            var prevFrom = fromDate.AddDays(-span.TotalDays);
            return GetKpis(prevFrom, prevTo);
        }

        /// <summary>
        /// Get alerts for products with low stock and underperforming products.
        /// </summary>
        public DataTable GetAlerts()
        {
            string sql = @"SELECT 'Sắp hết hàng' AS AlertType, ProductCode, ProductName, 
                           Quantity AS CurrentValue, 'Số lượng tồn kho ≤ 5' AS Detail
                           FROM Products WHERE Quantity <= 5 AND Quantity > 0
                           UNION ALL
                           SELECT 'Hết hàng' AS AlertType, ProductCode, ProductName, 
                           Quantity AS CurrentValue, 'Cần nhập thêm ngay' AS Detail
                           FROM Products WHERE Quantity = 0
                           ORDER BY CurrentValue ASC
                           OFFSET 0 ROWS FETCH NEXT 15 ROWS ONLY";
            return DAL.DbHelper.ExecuteQuery(sql);
        }

        /// <summary>
        /// Get list of employees who created sales orders (for filter dropdown).
        /// </summary>
        public DataTable GetSalesEmployees()
        {
            string sql = @"SELECT DISTINCT e.EmployeeId, e.FullName 
                           FROM Employees e 
                           INNER JOIN SalesOrders s ON s.CreatedBy = e.EmployeeId
                           ORDER BY e.FullName";
            return DAL.DbHelper.ExecuteQuery(sql);
        }
    }

    /// <summary>
    /// Aggregate KPI data for a date range.
    /// </summary>
    public class ReportKpiData
    {
        public decimal TotalRevenue { get; set; }
        public int OrderCount { get; set; }
        public decimal AvgOrderValue { get; set; }

        /// <summary>
        /// Calculate growth percentage compared to another period.
        /// Returns null if the comparison period has zero revenue.
        /// </summary>
        public decimal? CalculateGrowthPercent(ReportKpiData previousPeriod)
        {
            if (previousPeriod == null || previousPeriod.TotalRevenue == 0) return null;
            return ((TotalRevenue - previousPeriod.TotalRevenue) / previousPeriod.TotalRevenue) * 100m;
        }
    }
}
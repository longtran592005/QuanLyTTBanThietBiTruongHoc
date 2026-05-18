using System;
using System.Data;
using System.Data.SQLite;

namespace BLL
{
    public class ReportService
    {
        public DataTable GetRevenueByDay(DateTime fromDate, DateTime toDate)
        {
            string sql = @"SELECT date(OrderDate) AS ReportDate,
SUM(TotalAmount) AS Revenue,
SUM(SubTotal) AS SubTotal,
SUM(Discount) AS Discount,
SUM(VAT) AS VAT
FROM SalesOrders
WHERE date(OrderDate) >= date(@fromDate) AND date(OrderDate) < date(@toDate, '+1 day')
GROUP BY date(OrderDate)
ORDER BY ReportDate";
            return DAL.DbHelper.ExecuteQuery(sql,
                new SQLiteParameter("@fromDate", fromDate.Date),
                new SQLiteParameter("@toDate", toDate.Date));
        }

        public DataTable GetTopProducts(DateTime fromDate, DateTime toDate)
        {
            string sql = @"SELECT p.ProductCode, p.ProductName, SUM(d.Quantity) AS SoldQuantity,
SUM(d.Quantity * d.UnitPrice) AS SalesAmount
FROM SalesOrderDetails d
INNER JOIN SalesOrders s ON s.SalesOrderId = d.SalesOrderId
INNER JOIN Products p ON p.ProductId = d.ProductId
WHERE date(s.OrderDate) >= date(@fromDate) AND date(s.OrderDate) < date(@toDate, '+1 day')
GROUP BY p.ProductCode, p.ProductName
ORDER BY SoldQuantity DESC
LIMIT 10";
            return DAL.DbHelper.ExecuteQuery(sql,
                new SQLiteParameter("@fromDate", fromDate.Date),
                new SQLiteParameter("@toDate", toDate.Date));
        }
    }
}
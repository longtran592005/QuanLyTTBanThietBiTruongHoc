using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Data.SqlClient;
using DTO;

namespace DAL.Repositories
{
    public class SalesRepository
    {
        private static bool IsSqlServerConnection(string connectionString)
        {
            return !string.IsNullOrWhiteSpace(connectionString) &&
                   (connectionString.IndexOf("Initial Catalog=", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    connectionString.IndexOf("Integrated Security=", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    connectionString.IndexOf("Trusted_Connection=", StringComparison.OrdinalIgnoreCase) >= 0);
        }

        public int CreateInvoiceWithConnection(string connectionString, int? customerId, int createdBy, decimal discount, decimal vatPercent, List<SalesCartItem> items)
        {
            if (items == null || items.Count == 0) throw new ArgumentException("Invoice must contain at least one item.");

            var subtotal = 0m;
            foreach (var item in items)
                subtotal += item.LineTotal;

            var vatAmount = subtotal * vatPercent / 100m;
            var totalAmount = subtotal - discount + vatAmount;

            if (IsSqlServerConnection(connectionString))
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (var tx = conn.BeginTransaction())
                    {
                        try
                        {
                            var orderCode = "SO" + DateTime.Now.ToString("yyyyMMddHHmmss");
                            int orderId;
                            using (var cmd = new SqlCommand(@"INSERT INTO SalesOrders (SalesOrderCode, CustomerId, CreatedBy, OrderDate, SubTotal, Discount, VAT, TotalAmount)
VALUES (@code, @customerId, @createdBy, CURRENT_TIMESTAMP, @subtotal, @discount, @vat, @total); SELECT CAST(SCOPE_IDENTITY() AS int);", conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@code", orderCode);
                                cmd.Parameters.AddWithValue("@customerId", (object)customerId ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@createdBy", createdBy);
                                cmd.Parameters.AddWithValue("@subtotal", subtotal);
                                cmd.Parameters.AddWithValue("@discount", discount);
                                cmd.Parameters.AddWithValue("@vat", vatAmount);
                                cmd.Parameters.AddWithValue("@total", totalAmount);
                                orderId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            foreach (var item in items)
                            {
                                using (var cmdDetail = new SqlCommand(@"INSERT INTO SalesOrderDetails (SalesOrderId, ProductId, Quantity, UnitPrice)
VALUES (@orderId, @productId, @quantity, @unitPrice);", conn, tx))
                                {
                                    cmdDetail.Parameters.AddWithValue("@orderId", orderId);
                                    cmdDetail.Parameters.AddWithValue("@productId", item.ProductId);
                                    cmdDetail.Parameters.AddWithValue("@quantity", item.Quantity);
                                    cmdDetail.Parameters.AddWithValue("@unitPrice", item.UnitPrice);
                                    cmdDetail.ExecuteNonQuery();
                                }

                                using (var cmdStock = new SqlCommand(@"UPDATE Products SET Quantity = Quantity - @quantity WHERE ProductId = @productId AND Quantity >= @quantity;", conn, tx))
                                {
                                    cmdStock.Parameters.AddWithValue("@quantity", item.Quantity);
                                    cmdStock.Parameters.AddWithValue("@productId", item.ProductId);
                                    var affected = cmdStock.ExecuteNonQuery();
                                    if (affected == 0)
                                        throw new InvalidOperationException("Insufficient stock for product ID " + item.ProductId);
                                }

                                var inventoryRepo = new InventoryLogRepository();
                                inventoryRepo.RecordChange(conn, tx, item.ProductId, -item.Quantity, "Bán hàng " + orderCode, createdBy);
                            }

                            tx.Commit();
                            return orderId;
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
                }
            }

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        var orderCode = "SO" + DateTime.Now.ToString("yyyyMMddHHmmss");
                        int orderId;
                        using (var cmd = new SQLiteCommand(@"INSERT INTO SalesOrders (SalesOrderCode, CustomerId, CreatedBy, OrderDate, SubTotal, Discount, VAT, TotalAmount)
VALUES (@code, @customerId, @createdBy, CURRENT_TIMESTAMP, @subtotal, @discount, @vat, @total); SELECT last_insert_rowid();", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@code", orderCode);
                            cmd.Parameters.AddWithValue("@customerId", (object)customerId ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@createdBy", createdBy);
                            cmd.Parameters.AddWithValue("@subtotal", subtotal);
                            cmd.Parameters.AddWithValue("@discount", discount);
                            cmd.Parameters.AddWithValue("@vat", vatAmount);
                            cmd.Parameters.AddWithValue("@total", totalAmount);
                            orderId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        foreach (var item in items)
                        {
                            using (var cmdDetail = new SQLiteCommand(@"INSERT INTO SalesOrderDetails (SalesOrderId, ProductId, Quantity, UnitPrice)
VALUES (@orderId, @productId, @quantity, @unitPrice);", conn, tx))
                            {
                                cmdDetail.Parameters.AddWithValue("@orderId", orderId);
                                cmdDetail.Parameters.AddWithValue("@productId", item.ProductId);
                                cmdDetail.Parameters.AddWithValue("@quantity", item.Quantity);
                                cmdDetail.Parameters.AddWithValue("@unitPrice", item.UnitPrice);
                                cmdDetail.ExecuteNonQuery();
                            }

                            using (var cmdStock = new SQLiteCommand(@"UPDATE Products SET Quantity = Quantity - @quantity WHERE ProductId = @productId AND Quantity >= @quantity;", conn, tx))
                            {
                                cmdStock.Parameters.AddWithValue("@quantity", item.Quantity);
                                cmdStock.Parameters.AddWithValue("@productId", item.ProductId);
                                var affected = cmdStock.ExecuteNonQuery();
                                if (affected == 0)
                                    throw new InvalidOperationException("Insufficient stock for product ID " + item.ProductId);
                            }

                            var inventoryRepo = new InventoryLogRepository();
                            inventoryRepo.RecordChange(conn, tx, item.ProductId, -item.Quantity, "Bán hàng " + orderCode, createdBy);
                        }

                        tx.Commit();
                        return orderId;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
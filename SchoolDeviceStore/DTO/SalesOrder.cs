using System;

namespace DTO
{
    public class SalesOrder
    {
        public int SalesOrderId { get; set; }
        public string SalesOrderCode { get; set; }
        public int? CustomerId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal VAT { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
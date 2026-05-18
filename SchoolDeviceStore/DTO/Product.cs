namespace DTO
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int? CategoryId { get; set; }
        public int? ManufacturerId { get; set; }
        public int? SupplierId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }
}

using System;

namespace DTO
{
    public class InventoryLog
    {
        public int InventoryLogId { get; set; }
        public int ProductId { get; set; }
        public int Change { get; set; }
        public string Reason { get; set; }
        public int? ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}
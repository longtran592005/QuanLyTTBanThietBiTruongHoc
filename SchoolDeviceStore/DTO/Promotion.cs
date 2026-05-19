using System;

namespace DTO
{
    /// <summary>
    /// Data Transfer Object for Promotion / Discount Campaign
    /// </summary>
    public class Promotion
    {
        public int PromotionId { get; set; }
        public string PromotionCode { get; set; }
        public string PromotionName { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// 'Percentage' or 'FixedAmount'
        /// </summary>
        public string DiscountType { get; set; }

        /// <summary>
        /// Discount value: e.g. 10 means 10% or 500000 means 500,000 VND
        /// </summary>
        public decimal DiscountValue { get; set; }

        /// <summary>
        /// Minimum order amount required to apply this promotion
        /// </summary>
        public decimal MinOrderAmount { get; set; }

        /// <summary>
        /// Maximum discount amount (for percentage discounts). NULL means no cap.
        /// </summary>
        public decimal? MaxDiscountAmount { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Maximum number of times this promotion can be used. NULL means unlimited.
        /// </summary>
        public int? UsageLimit { get; set; }

        /// <summary>
        /// Number of times this promotion has been used
        /// </summary>
        public int UsageCount { get; set; }

        public bool IsActive { get; set; }

        /// <summary>
        /// 'All', 'Category', or 'Product'
        /// </summary>
        public string AppliesTo { get; set; }

        /// <summary>
        /// CategoryId or ProductId when AppliesTo is not 'All'
        /// </summary>
        public int? ApplyTargetId { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Returns true if this promotion is currently valid (active, within date range, usage not exhausted)
        /// </summary>
        public bool IsCurrentlyValid
        {
            get
            {
                if (!IsActive) return false;
                var now = DateTime.Now;
                if (now < StartDate || now > EndDate) return false;
                if (UsageLimit.HasValue && UsageCount >= UsageLimit.Value) return false;
                return true;
            }
        }

        /// <summary>
        /// Returns a display-friendly status string
        /// </summary>
        public string StatusDisplay
        {
            get
            {
                if (!IsActive) return "Ngừng hoạt động";
                var now = DateTime.Now;
                if (now < StartDate) return "Sắp diễn ra";
                if (now > EndDate) return "Đã hết hạn";
                if (UsageLimit.HasValue && UsageCount >= UsageLimit.Value) return "Đã hết lượt";
                return "Đang hoạt động";
            }
        }
    }
}

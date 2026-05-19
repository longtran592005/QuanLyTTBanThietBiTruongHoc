using System;
using System.Collections.Generic;
using System.Data;
using DAL.Repositories;
using DTO;

namespace BLL
{
    /// <summary>
    /// Business logic for Promotion management and discount calculation.
    /// </summary>
    public class PromotionService
    {
        private readonly PromotionRepository _repo = new PromotionRepository();

        public List<Promotion> GetAll()
        {
            return _repo.GetAll();
        }

        public DataTable GetAllAsDataTable()
        {
            return _repo.GetAllAsDataTable();
        }

        public Promotion GetById(int promotionId)
        {
            return _repo.GetById(promotionId);
        }

        public List<Promotion> GetActivePromotions()
        {
            return _repo.GetActivePromotions();
        }

        public int CreatePromotion(Promotion promo)
        {
            ValidatePromotion(promo);
            if (_repo.CodeExists(promo.PromotionCode))
                throw new ArgumentException($"Mã khuyến mãi '{promo.PromotionCode}' đã tồn tại.");
            return _repo.Create(promo);
        }

        public bool UpdatePromotion(Promotion promo)
        {
            ValidatePromotion(promo);
            if (_repo.CodeExists(promo.PromotionCode, promo.PromotionId))
                throw new ArgumentException($"Mã khuyến mãi '{promo.PromotionCode}' đã được sử dụng.");
            return _repo.Update(promo);
        }

        public bool DeletePromotion(int promotionId)
        {
            return _repo.Delete(promotionId);
        }

        /// <summary>
        /// Validate and calculate the discount amount for a given promotion code and subtotal.
        /// Returns null if the promotion is invalid or not applicable.
        /// </summary>
        public PromotionDiscountResult ValidateAndCalculate(string promotionCode, decimal subtotal)
        {
            if (string.IsNullOrWhiteSpace(promotionCode))
                return null;

            var promo = _repo.GetByCode(promotionCode.Trim().ToUpper());
            if (promo == null)
                return new PromotionDiscountResult { IsValid = false, ErrorMessage = "Mã khuyến mãi không tồn tại." };

            if (!promo.IsCurrentlyValid)
                return new PromotionDiscountResult { IsValid = false, ErrorMessage = $"Mã khuyến mãi '{promotionCode}' {promo.StatusDisplay.ToLower()}." };

            if (subtotal < promo.MinOrderAmount)
                return new PromotionDiscountResult
                {
                    IsValid = false,
                    ErrorMessage = $"Đơn hàng tối thiểu {promo.MinOrderAmount:N0} ₫ để áp dụng mã này."
                };

            decimal discountAmount;
            if (promo.DiscountType == "Percentage")
            {
                discountAmount = subtotal * promo.DiscountValue / 100m;
                if (promo.MaxDiscountAmount.HasValue && discountAmount > promo.MaxDiscountAmount.Value)
                    discountAmount = promo.MaxDiscountAmount.Value;
            }
            else
            {
                discountAmount = promo.DiscountValue;
            }

            // Discount cannot exceed subtotal
            if (discountAmount > subtotal)
                discountAmount = subtotal;

            return new PromotionDiscountResult
            {
                IsValid = true,
                Promotion = promo,
                DiscountAmount = discountAmount,
                Description = promo.DiscountType == "Percentage"
                    ? $"Giảm {promo.DiscountValue}% (tối đa {promo.MaxDiscountAmount?.ToString("N0") ?? "∞"} ₫)"
                    : $"Giảm {promo.DiscountValue:N0} ₫"
            };
        }

        /// <summary>
        /// Increment usage count after successful order creation.
        /// </summary>
        public void RecordUsage(int promotionId)
        {
            _repo.IncrementUsage(promotionId);
        }

        private void ValidatePromotion(Promotion promo)
        {
            if (string.IsNullOrWhiteSpace(promo.PromotionCode))
                throw new ArgumentException("Mã khuyến mãi không được để trống.");
            if (string.IsNullOrWhiteSpace(promo.PromotionName))
                throw new ArgumentException("Tên chương trình không được để trống.");
            if (promo.DiscountValue <= 0)
                throw new ArgumentException("Giá trị giảm giá phải lớn hơn 0.");
            if (promo.DiscountType == "Percentage" && promo.DiscountValue > 100)
                throw new ArgumentException("Phần trăm giảm giá không được vượt quá 100%.");
            if (promo.StartDate > promo.EndDate)
                throw new ArgumentException("Ngày bắt đầu phải trước ngày kết thúc.");
        }
    }

    /// <summary>
    /// Result of a promotion validation and discount calculation.
    /// </summary>
    public class PromotionDiscountResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public Promotion Promotion { get; set; }
        public decimal DiscountAmount { get; set; }
        public string Description { get; set; }
    }
}

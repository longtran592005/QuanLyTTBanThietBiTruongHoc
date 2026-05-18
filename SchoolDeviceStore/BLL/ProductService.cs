using System;
using System.Collections.Generic;
using DAL.Repositories;
using DTO;

namespace BLL
{
    /// <summary>
    /// Business logic for product management. Performs validation and coordinates DAL operations.
    /// </summary>
    public class ProductService
    {
        private readonly ProductRepository _repo = new ProductRepository();

        public List<Product> GetAll()
        {
            return _repo.GetAll();
        }

        public Product GetById(int id)
        {
            if (id <= 0) return null;
            return _repo.GetById(id);
        }

        public List<Product> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return _repo.GetAll();
            return _repo.Search(keyword.Trim());
        }

        public int Create(Product p)
        {
            ValidateProduct(p, isNew: true);
            if (_repo.ExistsByCode(p.ProductCode))
                throw new ArgumentException("Product code already exists.");
            return _repo.Create(p);
        }

        public bool Update(Product p)
        {
            ValidateProduct(p, isNew: false);
            if (p.ProductId <= 0) throw new ArgumentException("Invalid product id");
            if (_repo.ExistsByCodeExceptId(p.ProductCode, p.ProductId))
                throw new ArgumentException("Product code already exists.");
            return _repo.Update(p);
        }

        public bool Delete(int productId)
        {
            if (productId <= 0) throw new ArgumentException("Invalid product id");
            return _repo.Delete(productId);
        }

        private void ValidateProduct(Product p, bool isNew)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));
            ValidationHelper.RequireText(p.ProductCode, "Product code");
            ValidationHelper.RequireText(p.ProductName, "Product name");
            ValidationHelper.RequireNonNegativeDecimal(p.UnitPrice, "Unit price");
            ValidationHelper.RequireNonNegativeInt(p.Quantity, "Quantity");
        }
    }
}

using System;
using System.Collections.Generic;
using DAL.Repositories;
using DAL;
using DTO;

namespace BLL
{
    public class SalesService
    {
        private readonly SalesRepository _repo = new SalesRepository();

        public int CreateInvoice(int? customerId, int createdBy, decimal discount, decimal vatPercent, List<SalesCartItem> items)
        {
            if (createdBy <= 0) throw new ArgumentException("CreatedBy is required.");
            if (items == null || items.Count == 0) throw new ArgumentException("At least one item is required.");

            // Directly read connection string using DbHelper (moved ConfigurationManager dependency out of BLL)
            var cs = DbHelper.GetConnectionString();
            if (string.IsNullOrWhiteSpace(cs))
                throw new InvalidOperationException("Missing SchoolDeviceStoreDB connection string.");

            return _repo.CreateInvoiceWithConnection(cs, customerId, createdBy, discount, vatPercent, items);
        }
    }
}
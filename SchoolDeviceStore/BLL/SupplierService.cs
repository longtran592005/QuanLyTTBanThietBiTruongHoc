using System.Collections.Generic;
using DAL.Repositories;
using DTO;

namespace BLL
{
    public class SupplierService
    {
        private readonly SupplierRepository _repo = new SupplierRepository();
        public List<Supplier> GetAll() => _repo.GetAll();
        public Supplier GetById(int id) => _repo.GetById(id);
        public int Create(Supplier supplier) => _repo.Create(supplier);
        public bool Update(Supplier supplier) => _repo.Update(supplier);
        public bool Delete(int id) => _repo.Delete(id);
    }
}

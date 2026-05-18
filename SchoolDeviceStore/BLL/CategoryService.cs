using System.Collections.Generic;
using DAL.Repositories;
using DTO;

namespace BLL
{
    public class CategoryService
    {
        private readonly CategoryRepository _repo = new CategoryRepository();
        public List<Category> GetAll() => _repo.GetAll();
        public Category GetById(int id) => _repo.GetById(id);
        public int Create(Category category) => _repo.Create(category);
        public bool Update(Category category) => _repo.Update(category);
        public bool Delete(int id) => _repo.Delete(id);
    }
}

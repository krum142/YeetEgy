using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yeetegy.Data.Common.Repositories;
using Yeetegy.Data.Models;
using Yeetegy.Services.Data.Interfaces;

namespace Yeetegy.Services.Data
{
    public class CategoryService : ICategoryService
    {
        private readonly IDeletableEntityRepository<Category> categoryRepository;

        public CategoryService(IDeletableEntityRepository<Category> categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        public async Task CreateAsync(string name)
        {
            var category = new Category()
            {
                Name = name,
            };

            await this.categoryRepository.AddAsync(category);
            await this.categoryRepository.SaveChangesAsync();
        }

        public IEnumerable<string> GetAll()
        {
            var categories = this.categoryRepository.AllAsNoTracking().Select(c => c.Name).ToList();

            return categories;
        }

        public string GetId(string name)
        {
            var category = this.categoryRepository
                .AllAsNoTracking()
                .Where(c => c.Name == name)
                .Select(c => c.Id)
                .FirstOrDefault();

            return category;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Yeetegy.Data.Common.Repositories;
using Yeetegy.Data.Models;
using Yeetegy.Services.Data.Interfaces;
using Yeetegy.Web.ViewModels;

namespace Yeetegy.Services.Data
{
    public class CategoryService : ICategoryService
    {
        private readonly IDeletableEntityRepository<Category> categoryRepository;
        private readonly ICloudinaryService cloudinaryService;

        public CategoryService(IDeletableEntityRepository<Category> categoryRepository, ICloudinaryService cloudinaryService)
        {
            this.categoryRepository = categoryRepository;
            this.cloudinaryService = cloudinaryService;
        }

        public async Task CreateAsync(string name, IFormFile image)
        {
            var imgUrl = this.cloudinaryService.SaveCloudinary(image);

            var category = new Category()
            {
                ImageUrl = imgUrl.Result,
                Name = name,
            };

            await this.categoryRepository.AddAsync(category);
            await this.categoryRepository.SaveChangesAsync();

        }

        public IEnumerable<CategoryViewModel> GetAll()
        {
            var categories = this.categoryRepository
                .AllAsNoTracking()
                .Select(c => new CategoryViewModel
            {
                    Name = c.Name,
                    ImageUrl = c.ImageUrl,
            }).ToList();

            return categories;
        }

        public async Task DeleteAsync(string name)
        {
            var category = this.categoryRepository.All().FirstOrDefault(x => x.Name == name);

            if (category != null)
            {
                this.categoryRepository.Delete(category);

                await this.categoryRepository.SaveChangesAsync();
            }

        }

        public bool IsThereAny(string categoryName)
        {
            return this.categoryRepository.AllAsNoTracking().Any(x => x.Name == categoryName);
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
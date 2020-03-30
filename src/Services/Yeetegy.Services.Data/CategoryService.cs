using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Yeetegy.Data.Common.Repositories;
using Yeetegy.Data.Models;
using Yeetegy.Services.Data.Interfaces;
using Yeetegy.Services.Mapping;

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
            var imgUrl = this.cloudinaryService.SaveCloudinaryAsync(image);

            var category = new Category()
            {
                ImageUrl = imgUrl.Result,
                Name = name,
            };

            await this.categoryRepository.AddAsync(category);
            await this.categoryRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>()
        {
            return await this.categoryRepository
                .AllAsNoTracking()
                .To<T>().ToListAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetAllListItemsAsync()
        {
            var categories = await this.categoryRepository
                .AllAsNoTracking()
                .Select(c => new SelectListItem()
                {
                    Text = c.Name,
                }).ToListAsync();

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

        public async Task<bool> IsThereAnyAsync(string categoryName)
        {
            return await this.categoryRepository.AllAsNoTracking().AnyAsync(x => x.Name == categoryName);
        }

        public async Task<string> GetIdAsync(string categoryName)
        {
            var categoryId = await this.categoryRepository.AllAsNoTracking()
                .Where(c => c.Name == categoryName)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            return categoryId;
        }

        public async Task<string> GetImgAsync(string categoryName)
        {
            var categoryImg = await this.categoryRepository
                .AllAsNoTracking()
                .Where(c => c.Name == categoryName)
                .Select(c => c.ImageUrl)
                .FirstOrDefaultAsync();

            return categoryImg;
        }
    }
}

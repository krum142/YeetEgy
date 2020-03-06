using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public CategorysViewModel GetAll()
        {
            var categories = this.categoryRepository
                .AllAsNoTracking()
                .Select(c => new CategoryViewModel
                {
                    Name = c.Name,
                    ImageUrl = c.ImageUrl,
                }).ToList();

            var model = new CategorysViewModel()
            {
                CategoryViewModels = categories,
            };
            return model;
        }

        public IEnumerable<SelectListItem> GetAllListItems()
        {
            var categories = this.categoryRepository
                .AllAsNoTracking()
                .Select(c => new SelectListItem()
                {
                    Text = c.Name,
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

        public string GetId(string categoryName)
        {
            var categoryId = this.categoryRepository.AllAsNoTracking()
                .Where(c => c.Name == categoryName)
                .Select(c => c.Id)
                .FirstOrDefault();

            return categoryId;
        }

        public string GetImg(string categoryName)
        {
            var categoryImg = this.categoryRepository
                .AllAsNoTracking()
                .Where(c => c.Name == categoryName)
                .Select(c => c.ImageUrl)
                .FirstOrDefault();

            return categoryImg;
        }
    }
}
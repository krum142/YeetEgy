using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Yeetegy.Common;
using Yeetegy.Services.Data.Interfaces;
using Yeetegy.Web.ViewModels;

namespace Yeetegy.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ICategoryService categoryService;

        public HomeController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        public async Task<IActionResult> Index(string category)
        {
            if (category == null)
            {
                category = "Newest";
            }

            if (GlobalConstants.ConstantCategories.ContainsKey(category))
            {
                var categories = await this.categoryService.GetAllAsync<CategoryViewModel>();
                this.ViewData["CategoryName"] = GlobalConstants.ConstantCategories.FirstOrDefault(x => x.Key.ToLower() == category.ToLower()).Key;
                this.ViewData["CategoryUrl"] = GlobalConstants.ConstantCategories.FirstOrDefault(x => x.Key.ToLower() == category.ToLower()).Value;
                return this.View(categories);
            }

            if (await this.categoryService.IsThereAnyAsync(category))
            {
                var categories = await this.categoryService.GetAllAsync<CategoryViewModel>();
                var currentCat = categories.First(x => x.Name == category);
                this.ViewData["CategoryName"] = currentCat.Name;
                this.ViewData["CategoryUrl"] = currentCat.ImageUrl;
                return this.View(categories);
            }

            return this.Redirect("/Home/HttpError?statusCode=404");
        }

        public IActionResult HttpError(int statusCode)
        {
            return this.View(statusCode);
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}

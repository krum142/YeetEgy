using Yeetegy.Services.Data.Interfaces;

namespace Yeetegy.Web.Controllers
{
    using System.Diagnostics;

    using ViewModels;

    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private readonly ICategoryService categoryService;

        public HomeController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        public IActionResult Index(string category)
        {
            var categories = this.categoryService.GetAll();
            this.Response.Cookies.Append("IdCookie", "0");

            if (category == null)
            {
                return this.View(categories); // you gotta return some 404
            }

            if (this.categoryService.IsThereAny(category))
            {
                var currentCat = categoryService.GetImg(category);
                categories.CurrentName = category;
                categories.CurrentUrl = currentCat;
                return this.View(categories);
            }

            return this.View(categories);

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}

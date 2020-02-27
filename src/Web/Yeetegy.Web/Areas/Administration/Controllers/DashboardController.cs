using System.Collections.Generic;
using System.Threading.Tasks;
using Yeetegy.Services.Data.Interfaces;

namespace Yeetegy.Web.Areas.Administration.Controllers
{
    using Yeetegy.Services.Data;
    using Yeetegy.Web.ViewModels.Administration.Dashboard;

    using Microsoft.AspNetCore.Mvc;

    public class DashboardController : AdministrationController
    {
        private readonly ISettingsService settingsService;
        private readonly ICategoryService categoryService;
        private readonly IList<string> AllowedMimeFiles = new List<string>()
        {
            "image/apng",
            "image/bmp",
            "image/gif",
            "image/jpeg",
            "image/png"
        };

        public DashboardController(ISettingsService settingsService, ICategoryService categoryService)
        {
            this.settingsService = settingsService;
            this.categoryService = categoryService;
        }

        public IActionResult Index()
        {
            var viewModel = new IndexViewModel { SettingsCount = settingsService.GetCount(), };
            return this.View(viewModel);
        }

        public IActionResult CreateCategory()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(AddCategoryViewModel model)
        {
            if (ModelState.IsValid /*&& AllowedMimeFiles.Contains(model.File.ContentType)*/)
            {
               await this.categoryService.CreateAsync(model.Category);
            }

            return Redirect("/");
        }
    }
}

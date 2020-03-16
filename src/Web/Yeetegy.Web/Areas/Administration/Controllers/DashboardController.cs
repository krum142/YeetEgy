using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yeetegy.Common;
using Yeetegy.Services.Data.Interfaces;
using Yeetegy.Web.ViewModels.Administration.Dashboard;

namespace Yeetegy.Web.Areas.Administration.Controllers
{
    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    public class DashboardController : AdministrationController
    {
        private readonly ISettingsService settingsService;
        private readonly ICategoryService categoryService;

        public DashboardController(ISettingsService settingsService, ICategoryService categoryService)
        {
            this.settingsService = settingsService;
            this.categoryService = categoryService;
        }

        public IActionResult Index()
        {
            var viewModel = new IndexViewModel { SettingsCount = this.settingsService.GetCount(), };
            return this.View(viewModel);
        }

        public IActionResult CreateCategory()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(AddCategoryViewModel model)
        {
            if (model.Category != null)
            {
                if (this.ModelState.IsValid)
                {
                    await this.categoryService.CreateAsync(model.Category, model.File);
                    return this.Redirect("/");
                }

            }

            return this.View(model);
        }

        public async Task<IActionResult> DeleteCategory()
        {
            var model = await this.categoryService.GetAllListItemsAsync();
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(string category)
        {
            var isThereAnyCategory = await this.categoryService.IsThereAnyAsync(category);

            if (isThereAnyCategory)
            {
                await this.categoryService.DeleteAsync(category);
            }

            return this.View();
        }
    }
}

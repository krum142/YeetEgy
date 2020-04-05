using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Yeetegy.Services.Data.Interfaces;
using Yeetegy.Web.ViewModels;
using Yeetegy.Web.ViewModels.PostViewModels;

namespace Yeetegy.Web.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        private readonly ICategoryService categoryService;
        private readonly IUserService userService;

        public HomeController(
            ICategoryService categoryService,
            IUserService userService)
        {
            this.categoryService = categoryService;
            this.userService = userService;
        }

        [Route("/User/{username?}")]
        [Route("/User/{username?}/Posts")]
        [Route("/User/{username?}/Liked")]
        [Route("/User/{username?}/Comments")]
        public async Task<IActionResult> Index(string username = null)
        {
            var path = this.Request.Path.Value.Split("/");

            var typeSearch = path.Length == 4 ? path[3] : path.Length == 3 ? "Posts" : null;

            if (!string.IsNullOrWhiteSpace(username) && await this.userService.ExistsAsync(username) && typeSearch != null)
            {
                if (this.User.Identity.Name != username && typeSearch == "Liked")
                {
                    return this.NotFound();
                }

                var userContents = await this.userService.GetUserByNameAsync<UserPageContentsModel>(username);
                this.ViewData["Username"] = userContents.Username;
                this.ViewData["AvatarUrl"] = userContents.AvatarUrl;

                var categories = await this.categoryService.GetAllAsync<CategoryViewModel>();
                return this.View(categories);
            }

            return this.NotFound(); // 404
        }
    }
}

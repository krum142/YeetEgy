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
        private readonly IPostsService postsService;

        public HomeController(
            ICategoryService categoryService,
            IUserService userService,
            IPostsService postsService)
        {
            this.categoryService = categoryService;
            this.userService = userService;
            this.postsService = postsService;
        }

        [Route("/User/{username?}")]
        [Route("/User/{username?}/Posts")]
        [Route("/User/{username?}/Liked")]
        [Route("/User/{username?}/Comments")]
        public async Task<IActionResult> Index(string username = null)
        {
            var path = this.Request.Path.Value.Split("/");

            var typeSearch = path.Length == 4 ? path[3] : path.Length == 3 ? "Posts" : null;

            if (await this.userService.ExistsAsync(username) && typeSearch != null)
            {
                var categories = await this.categoryService.GetAllAsync<CategoryViewModel>();
                var userId = await this.userService.GetIdAsync(username);
                var userPosts = typeSearch switch
                {
                    "Liked" => await this.postsService.GetUserLikedAsync<PostsViewModel>(0, 5, userId),
                    "Comments" => await this.postsService.GetUserCommentedAsync<PostsViewModel>(0, 5, userId),
                    "Posts" => await this.postsService.GetUserPostsAsync<PostsViewModel>(0, 5, userId),
                    _ => await this.postsService.GetUserPostsAsync<PostsViewModel>(0, 5, userId)
                };

                var x = new UserPostsViewModel()
                {
                    Categorys = categories,
                    Posts = userPosts,
                };

                return this.View(categories);
            }
            else
            {
                return this.NotFound(); // 404
            }
        }
    }
}

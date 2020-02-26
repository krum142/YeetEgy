using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using Yeetegy.Services.Data.Interfaces;
using Yeetegy.Web.ViewModels;
using Yeetegy.Web.ViewModels.PostViewModels;

namespace Yeetegy.Web.Controllers
{
    public class PostsController : Controller
    {
        private readonly IPostsService postsService;
        private readonly ICategoryService categoryService;
        private readonly IConfiguration configuration;
        private int count;

        public PostsController(IPostsService postsService, ICategoryService categoryService, IConfiguration configuration)
        {
            this.postsService = postsService;
            this.categoryService = categoryService;
            this.configuration = configuration;
        }

        public IActionResult GetPost()
        {

            var cookie = this.Request.Cookies.FirstOrDefault(c => c.Key == "IdCookie");

            var posts = postsService.GetFivePosts(int.Parse(cookie.Value));

            if (posts.Any())
            {
                this.Response.Cookies.Append("IdCookie", (int.Parse(cookie.Value) + 5).ToString());
            }

            return this.Json(JsonConvert.SerializeObject(posts));
        }

        [Authorize]
        public async Task<IActionResult> Add()
        {
            await categoryService.CreateAsync("my test category");

            var category = categoryService.GetAll();

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddPostsViewModel post)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await postsService.CreatePostAsync(post, user, configuration["CloudSettings"]);

            return Redirect("/");
        }

    }
}
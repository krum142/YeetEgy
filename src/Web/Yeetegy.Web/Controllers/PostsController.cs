using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly IList<string> AllowedMimeFiles = new List<string>()
        {
            "image/apng",
            "image/bmp",
            "image/gif",
            "image/jpeg",
            "image/png"
        };

        public PostsController(IPostsService postsService, ICategoryService categoryService)
        {
            this.postsService = postsService;
            this.categoryService = categoryService;
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
            var category = categoryService.GetAll();

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddPostsViewModel post)
        {
            if (post.File != null && post.Tittle != null)
            {
                var checkCategory = categoryService.IsThereAny(post.Category);
                var fileContentType = AllowedMimeFiles.Contains(post.File.ContentType);

                if (fileContentType && checkCategory && ModelState.IsValid)
                {
                    var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    await postsService.CreatePostAsync(post, user);

                    return Redirect("/");
                }
            }
            
            return this.Content("A wrong file type Implement me!!!");
        }

    }
}
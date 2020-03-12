using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Yeetegy.Common;
using Yeetegy.Services.Data.Interfaces;
using Yeetegy.Web.ViewModels;
using Yeetegy.Web.ViewModels.PostViewModels;

namespace Yeetegy.Web.Controllers
{
    public class PostsController : Controller
    {
        private readonly IPostsService postsService;
        private readonly ICategoryService categoryService;

        public PostsController(IPostsService postsService, ICategoryService categoryService)
        {
            this.postsService = postsService;
            this.categoryService = categoryService;
        }

        public async Task<IActionResult> GetPost(int page)
        {
            var loadPostsCount = GlobalConstants.LoadPostCountAjax;
            var header = this.Request.Headers.FirstOrDefault(x => x.Key == "x-category").Value.ToString().Trim();

            header = string.IsNullOrWhiteSpace(header) ? "Newest" : header;

            if (GlobalConstants.ConstantCategories.Any(x => x.Key == header))
            {
                var posts = header switch
                {
                    "Newest" => this.postsService.GetPosts<PostsViewModel>(page, loadPostsCount),
                    "Popular" => this.postsService.GetPostsPopular<PostsViewModel>(page, loadPostsCount),
                    "Discussed" => this.postsService.GetPostsTrending<PostsViewModel>(page, loadPostsCount),
                    _ => this.postsService.GetPosts<PostsViewModel>(page, loadPostsCount),
                };
                return this.Json(JsonConvert.SerializeObject(posts));
            }

            if (await this.categoryService.IsThereAnyAsync(header))
            {
                var posts = this.postsService.GetPosts<PostsViewModel>(page, loadPostsCount, header);
                return this.Json(JsonConvert.SerializeObject(posts));
            }

            return this.Json(JsonConvert.SerializeObject(new List<PostsViewModel>()));
        }

        public async Task<IActionResult> Like(string id)
        {

            if (this.User.Identity.IsAuthenticated)
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                await this.postsService.LikePostAsync(id,userId);

                return this.Ok();
            }

            return this.NoContent();
        }

        [Authorize]
        public IActionResult Add()
        {
            var model = new AddPostsViewModel()
            {
                Categorys = this.categoryService.GetAllListItems(),
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddPostsViewModel post)
        {
            if (post.File != null && post.Tittle != null)
            {
                var checkCategory = await this.categoryService.IsThereAnyAsync(post.Category);

                if (checkCategory && this.ModelState.IsValid)
                {
                    var user = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    await this.postsService.CreatePostAsync(post, user);

                    return this.Redirect("/");
                }
            }

            post.Categorys = this.categoryService.GetAllListItems();

            return this.View(post);
        }
    }
}
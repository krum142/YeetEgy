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

        public async Task<IActionResult> GetPosts(int page)
        {
            var loadPostsCount = GlobalConstants.LoadPostCountAjax;
            var header = this.Request.Headers.FirstOrDefault(x => x.Key == "x-category").Value.ToString().Trim();

            header = string.IsNullOrWhiteSpace(header) ? "Newest" : header;

            if (GlobalConstants.ConstantCategories.Any(x => x.Key == header))
            {
                var posts = header switch
                {
                    "Newest" => await this.postsService.GetPostsAsync<PostsViewModel>(page, loadPostsCount),
                    "Popular" => await this.postsService.GetPostsPopularAsync<PostsViewModel>(page, loadPostsCount),
                    "Discussed" => await this.postsService.GetPostsTrendingAsync<PostsViewModel>(page, loadPostsCount),
                    _ => await this.postsService.GetPostsAsync<PostsViewModel>(page, loadPostsCount),
                };
                return this.Json(posts);
            }

            if (await this.categoryService.IsThereAnyAsync(header))
            {
                var posts = await this.postsService.GetPostsAsync<PostsViewModel>(page, loadPostsCount, header);
                return this.Json(posts);
            }

            return this.Json(new List<PostsViewModel>());
        }

        public async Task<IActionResult> PostDetails(string id)
        {
            if (await this.postsService.DoesPostExistAsync(id))
            {
                var post = await this.postsService.GetPostAsync<PostCommentsViewModel>(id);
                var category = await this.categoryService.GetAllAsync<CategoryViewModel>();

                var details = new PostDetailsViewModel()
                {
                    PostViewModel = post,
                    CategoryViewModel = category,
                };

                return this.View(details);
            }

            return this.NotFound();
        }

        public async Task<IActionResult> Vote(string id, string vote)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await this.postsService.DoesPostExistAsync(id))
            {
                return this.NotFound();
            }

            if (this.User.Identity.IsAuthenticated)
            {
                var lastValue = await this.postsService.GetPostVoteValueAsync(id, userId);
                if (lastValue != null)
                {
                    switch (vote)
                    {
                        case "Like":
                            switch (lastValue)
                            {
                                case "Like":
                                    await this.postsService.UndoLikeAsync(id, userId);
                                    return this.NoContent();
                                case "Dislike":
                                    await this.postsService.DislikeToLikeAsync(id, userId);
                                    return this.Accepted();
                                default: return this.NotFound();
                            }

                        case "Dislike":
                            switch (lastValue)
                            {
                                case "Dislike":
                                    await this.postsService.UndoDislikeAsync(id, userId);
                                    return this.NoContent();
                                case "Like":
                                    await this.postsService.LikeToDislikeAsync(id, userId);
                                    return this.Accepted();
                                default: return this.NotFound();
                            }

                        default: return this.NotFound();
                    }
                }

                switch (vote)
                {
                    case "Like":
                        await this.postsService.LikeAsync(id, userId);
                        return this.Ok();
                    case "Dislike":
                        await this.postsService.DislikeAsync(id, userId);
                        return this.Ok();
                    default: return this.NotFound();
                }
            }

            return this.Unauthorized();
        }

        [Authorize]
        public async Task<IActionResult> Add()
        {
            var model = new AddPostsViewModel()
            {
                Categorys = await this.categoryService.GetAllListItemsAsync(),
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

            post.Categorys = await this.categoryService.GetAllListItemsAsync();

            return this.View(post);
        }
    }
}

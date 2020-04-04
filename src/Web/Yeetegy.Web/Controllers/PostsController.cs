﻿using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> GetPosts(int page, string category)
        {
            var loadPostsCount = GlobalConstants.LoadPostCountAjax;

            var currentCategory = string.IsNullOrWhiteSpace(category) ? "newest" : category.ToLower();

            if (GlobalConstants.ConstantCategories.Any(x => x.Key.ToLower() == currentCategory))
            {
                var posts = currentCategory switch
                {
                    "newest" => await this.postsService.GetPostsAsync<PostsViewModel>(page, loadPostsCount),
                    "popular" => await this.postsService.GetPostsPopularAsync<PostsViewModel>(page, loadPostsCount),
                    "discussed" => await this.postsService.GetPostsTrendingAsync<PostsViewModel>(page, loadPostsCount),
                    _ => await this.postsService.GetPostsAsync<PostsViewModel>(page, loadPostsCount),
                };
                return this.PartialView("_GetPostsPartial", posts);
            }

            if (await this.categoryService.IsThereAnyAsync(currentCategory))
            {
                var posts = await this.postsService.GetPostsAsync<PostsViewModel>(page, loadPostsCount, currentCategory);
                return this.PartialView("_GetPostsPartial", posts);
            }

            return this.Content(string.Empty);
        }

        public async Task<IActionResult> PostDetails(string id)
        {
            if (await this.postsService.DoesPostExistAsync(id))
            {
                var post = await this.postsService.GetPostAsync<PostsViewModel>(id);
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

        [Authorize]
        public async Task<IActionResult> Add()
        {
            var model = new AddPostsModel()
            {
                Categorys = await this.categoryService.GetAllListItemsAsync(),
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddPostsModel post)
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

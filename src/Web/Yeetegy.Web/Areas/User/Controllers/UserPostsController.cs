using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Yeetegy.Common;
using Yeetegy.Services.Data.Interfaces;
using Yeetegy.Web.ViewModels.PostViewModels;

namespace Yeetegy.Web.Areas.User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPostsController : Controller
    {
        private readonly IUserService userService;
        private readonly IPostsService postsService;

        public UserPostsController(IUserService userService, IPostsService postsService)
        {
            this.userService = userService;
            this.postsService = postsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserPosts(int page, string category, string username)
        {
            if (!string.IsNullOrWhiteSpace(username) && await this.userService.ExistsAsync(username))
            {
                var loadPostsCount = GlobalConstants.LoadUserPostsCountAjax;
                var userId = await this.userService.GetIdAsync(username);
                var currentUser = this.User.Identity.Name;

                if (currentUser != username && category == "Liked")
                {
                    return this.NotFound();
                }

                var userPosts = category switch
                {
                    "Liked" => await this.postsService.GetUserLikedAsync<PostsViewModel>(page, loadPostsCount, userId),
                    "Comments" => await this.postsService.GetUserCommentedAsync<PostsViewModel>(page, loadPostsCount, userId),
                    "Posts" => await this.postsService.GetUserPostsAsync<PostsViewModel>(page, loadPostsCount, userId),
                    _ => await this.postsService.GetUserPostsAsync<PostsViewModel>(page, loadPostsCount, userId)
                };

                return this.PartialView("_GetPostsPartial", userPosts);
            }

            return this.NotFound();
        }
    }
}

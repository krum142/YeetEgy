using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Yeetegy.Common;
using Yeetegy.Services.Data.Interfaces;
using Yeetegy.Web.ViewModels.CommentModels;

namespace Yeetegy.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : Controller // removed Base
    {
        private readonly ICommentsService commentsService;
        private readonly IPostsService postsService;

        public CommentsController(ICommentsService commentsService, IPostsService postsService)
        {
            this.commentsService = commentsService;
            this.postsService = postsService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string postId, int offset)
        {
            if (await this.postsService.DoesPostExistAsync(postId))
            {
                var comments = await this.commentsService.GetCommentsAsync<CommentsViewModel>(postId, offset, GlobalConstants.LoadCommentsCountAjax);

                return this.PartialView("_CommentsPartial", comments);
            }

            return this.NotFound();
        }

        // [Authorize]
        [HttpPost]
        public async Task<ActionResult<ResponseAddComment>> Post([FromForm]AddCommentsModel data)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                await this.commentsService.CreateCommentAsync(data, userId);

                return new ResponseAddComment() { Status = "Created" };
            }

            return this.Unauthorized();
        }

        [HttpDelete]
        public async Task<ActionResult> Delete([FromForm]DeleteCommentInputModel data)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var commentUserId = await this.commentsService.TakeAuthorIdAsync(data.Id);

            var commentUserIsCurrentUser = this.User.Identity.IsAuthenticated && userId == commentUserId;
            if (commentUserIsCurrentUser || this.User.IsInRole(GlobalConstants.AdministratorRoleName))
            {
                await this.commentsService.DeleteCommentAsync(data.Id);

                return this.Ok(data.Id);
            }

            return this.Unauthorized();
        }
    }
}

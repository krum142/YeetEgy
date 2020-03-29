using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Yeetegy.Services.Data.Interfaces;
using Yeetegy.Web.ViewModels;

namespace Yeetegy.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VotesController : ControllerBase
    {
        private readonly IPostsService postsService;
        private readonly IVotesService votesService;
        private readonly ICommentsService commentsService;

        public VotesController(
            IPostsService postsService,
            IVotesService votesService,
            ICommentsService commentsService)
        {
            this.postsService = postsService;
            this.votesService = votesService;
            this.commentsService = commentsService;
        }

        [HttpPost]
        [ActionName("Post")]
        public async Task<ActionResult<ResponseVoteModel>> Post([FromBody]PostVoteInputModel input)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                if (await this.postsService.DoesPostExistAsync(input.PostId))
                {
                    var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var status = await this.votesService.PostVoteAsync(input.PostId, userId, input.IsUpvote);

                    return new ResponseVoteModel() { Status = status };
                }

                return this.NotFound();
            }

            return this.Unauthorized();
        }

        [HttpPost]
        [ActionName("Comment")]
        public async Task<ActionResult<ResponseVoteModel>> Comment([FromBody]CommentVoteInputModel input)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                if (await this.commentsService.DoesCommentExistAsync(input.CommentId))
                {
                    var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var status = await this.commentsService.CommentVoteAsync(input.CommentId, userId, input.IsUpvote);

                    return new ResponseVoteModel() { Status = status };
                }

                return this.NotFound();
            }

            return this.Unauthorized();
        }
    }
}

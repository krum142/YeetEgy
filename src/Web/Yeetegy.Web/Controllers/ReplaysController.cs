using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Yeetegy.Services.Data.Interfaces;
using Yeetegy.Web.ViewModels.CommentModels;
using Yeetegy.Web.ViewModels.ReplayModels;

namespace Yeetegy.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReplaysController : Controller
    {
        private readonly IReplaysService replaysService;
        private readonly ICommentsService commentsService;

        public ReplaysController(IReplaysService replaysService, ICommentsService commentsService)
        {
            this.replaysService = replaysService;
            this.commentsService = commentsService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string commentId)
        {
            if (await this.commentsService.DoesCommentExistAsync(commentId))
            {
                var replays = await this.replaysService.AllAsync<ReplayViewModel>(commentId);

                return this.PartialView("_ReplaysPartial", replays);
                //return this.Json(replays);
            }

            return this.NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<ResponseAddComment>> Post([FromForm]AddReplayModel data)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                await this.replaysService.CreateReplayAsync(data, userId);

                return new ResponseAddComment() { Status = "Created" };
            }

            return this.Unauthorized();
        }
    }
}

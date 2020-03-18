using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yeetegy.Services.Data;
using Yeetegy.Services.Data.Interfaces;
using Yeetegy.Web.ViewModels;

namespace Yeetegy.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VotesController : ControllerBase
    {
        private readonly IPostsService postsService;
        private readonly IVotesService votesService;

        public VotesController(IPostsService postsService, IVotesService votesService)
        {
            this.postsService = postsService;
            this.votesService = votesService;
        }

        [HttpPost]
        public async Task<ActionResult<ResponseVoteModel>> Post(VoteInputModel input)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                if (await this.postsService.DoesPostExistAsync(input.PostId))
                {
                    var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var status = await this.votesService.VoteAsync(input.PostId, userId, input.IsUpvote);

                    return new ResponseVoteModel() { Status = status };
                }

                return this.NotFound();
            }

            return this.Unauthorized();
        }
    }
}

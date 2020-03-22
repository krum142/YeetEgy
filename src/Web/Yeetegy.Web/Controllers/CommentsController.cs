using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yeetegy.Services.Data.Interfaces;
using Yeetegy.Web.ViewModels;

namespace Yeetegy.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentsService commentsService;

        public CommentsController(ICommentsService commentsService)
        {
            this.commentsService = commentsService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ResponseAddComment>> Post([FromForm]AddCommentsModel data)
        {

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var status = await this.commentsService.CreateCommentAsync(data, userId);

            return new ResponseAddComment() { Status = status };
        }
    }
}

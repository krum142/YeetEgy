using Yeetegy.Data.Models;
using Yeetegy.Services.Mapping;

namespace Yeetegy.Web.ViewModels
{
    public class ReplayViewModel : IMapFrom<Replay>
    {

        public string ApplicationUserId { get; set; }

        public string Description { get; set; }

        public string CommentId { get; set; }

        public string ImgUrl { get; set; }

        public int Likes { get; set; }

        public int Dislikes { get; set; }
    }
}
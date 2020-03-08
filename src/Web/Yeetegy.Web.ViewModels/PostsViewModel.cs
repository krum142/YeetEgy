using Yeetegy.Data.Models;
using Yeetegy.Services.Mapping;

namespace Yeetegy.Web.ViewModels
{
    public class PostsViewModel : IMapFrom<Post>
    {
        public string Id { get; set; }

        public string Tittle { get; set; }

        public string ImgUrl { get; set; }

        public int Likes { get; set; }

        public int Dislikes { get; set; }

        public string CategoryId { get; set; }
    }
}
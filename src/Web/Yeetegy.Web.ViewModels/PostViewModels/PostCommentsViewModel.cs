using System.Collections.Generic;
using Yeetegy.Data.Models;
using Yeetegy.Services.Mapping;

namespace Yeetegy.Web.ViewModels.PostViewModels
{
    public class PostCommentsViewModel : IMapFrom<Post>
    {
        public string Id { get; set; }

        public string Tittle { get; set; }

        public string ImgUrl { get; set; }

        public int Likes { get; set; }

        public int Dislikes { get; set; }

        public string CategoryId { get; set; }

        public int CommentsCount { get; set; }

        public IEnumerable<CommentsViewModel> Comments { get; set; }
    }
}
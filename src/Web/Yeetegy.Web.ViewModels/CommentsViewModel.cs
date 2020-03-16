using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Yeetegy.Data.Models;
using Yeetegy.Services.Mapping;

namespace Yeetegy.Web.ViewModels
{
    public class CommentsViewModel : IMapFrom<Comment>
    {
        public string ApplicationUserId { get; set; }

        public string Description { get; set; }

        public string PostId { get; set; }

        public string ImgUrl { get; set; }

        public int Likes { get; set; }

        public int Dislikes { get; set; }
    }
}
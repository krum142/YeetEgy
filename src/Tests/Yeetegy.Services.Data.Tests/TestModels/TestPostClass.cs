using System.Collections.Generic;
using Yeetegy.Data.Models;
using Yeetegy.Services.Mapping;
using Yeetegy.Web.ViewModels;

namespace Yeetegy.Services.Data.Tests.TestModels
{
    public class TestPostClass : IMapFrom<Post>
    {
        public string Id { get; set; }

        public string ApplicationUserUserName { get; set; }

        public string Tittle { get; set; }

        public string ImgUrl { get; set; }

        public int Likes { get; set; }

        public int Dislikes { get; set; }

        public string CategoryName { get; set; }

        public int CommentsCount { get; set; }

        public string CategoryImageUrl { get; set; }

        //public ICollection<TagViewModel> PostTags { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Yeetegy.Data.Models;
using Yeetegy.Services.Mapping;

namespace Yeetegy.Web.ViewModels.PostViewModels
{
    public class PostsViewModel : IMapFrom<Post>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string Tittle { get; set; }

        public string ImgUrl { get; set; }

        public int Likes { get; set; }

        public int Dislikes { get; set; }

        public string CategoryName { get; set; }

        public int CommentsCount { get; set; }

        public string CategoryImageUrl { get; set; }

        public ICollection<TagViewModel> PostTags { get; set; }

        public string Time { get; set; }

        public void CreateMappings(IProfileExpression configuration) // this can be done at the users browser if it proves to be slow
        {
            configuration.CreateMap<Post, PostsViewModel>()
                .ForMember(
                    x => x.Time,
                    y => y.MapFrom(x =>
                        (DateTime.UtcNow - x.CreatedOn).Days >= 1 ? x.CreatedOn.ToString("M") :
                        (DateTime.UtcNow - x.CreatedOn).Hours <= 1 ? (DateTime.UtcNow - x.CreatedOn).Minutes + "m" :
                        (DateTime.UtcNow - x.CreatedOn).Hours + "h"));
        }
    }
}

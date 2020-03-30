using System;

using AutoMapper;
using Yeetegy.Data.Models;
using Yeetegy.Services.Mapping;

namespace Yeetegy.Web.ViewModels.ReplayModels
{
    public class ReplayViewModel : IMapFrom<Comment>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string ApplicationUserId { get; set; }

        public string ApplicationUserAvatarUrl { get; set; }

        public string ApplicationUserUsername { get; set; }

        public string Description { get; set; }

        public string ReplayId { get; set; }

        public string ImgUrl { get; set; }

        public int Likes { get; set; }

        public int Dislikes { get; set; }

        public string Time { get; set; }

        public void CreateMappings(IProfileExpression configuration) // this can be done at the users browser if it proves to be slow
        {
            configuration.CreateMap<Comment, ReplayViewModel>()
                .ForMember(
                    x => x.Time,
                    y => y.MapFrom(x =>
                        (DateTime.UtcNow - x.CreatedOn).Days >= 1 ? x.CreatedOn.ToString("M") :
                        (DateTime.UtcNow - x.CreatedOn).Hours <= 1 ? (DateTime.UtcNow - x.CreatedOn).Minutes + "m" :
                        (DateTime.UtcNow - x.CreatedOn).Hours + "h"));
        }
    }
}

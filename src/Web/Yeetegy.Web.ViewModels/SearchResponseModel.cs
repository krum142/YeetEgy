using AutoMapper;
using Yeetegy.Data.Models;
using Yeetegy.Services.Mapping;

namespace Yeetegy.Web.ViewModels
{
    public class SearchResponseModel : IMapFrom<Tag>, IHaveCustomMappings
    {
        public string Value { get; set; }

        public int Count { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Tag, SearchResponseModel>()
                .ForMember(x => x.Count, y => y.MapFrom(x => x.PostTags.Count));
        }
    }
}
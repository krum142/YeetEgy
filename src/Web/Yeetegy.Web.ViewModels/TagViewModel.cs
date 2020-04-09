using Yeetegy.Data.Models;
using Yeetegy.Services.Mapping;

namespace Yeetegy.Web.ViewModels
{
    public class TagViewModel : IMapFrom<PostTag>
    {
        public string TagValue { get; set; }
    }
}
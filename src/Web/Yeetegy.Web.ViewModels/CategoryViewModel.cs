using Yeetegy.Data.Models;
using Yeetegy.Services.Mapping;

namespace Yeetegy.Web.ViewModels
{
    public class CategoryViewModel : IMapFrom<Category>
    {
        public string Name { get; set; }

        public string ImageUrl { get; set; }
    }
}
using System.Collections.Generic;
using Yeetegy.Data.Models;
using Yeetegy.Services.Mapping;
using Yeetegy.Web.ViewModels.PostViewModels;

namespace Yeetegy.Web.ViewModels
{
    public class PostDetailsViewModel : IMapFrom<Post>
    {
        public PostCommentsViewModel PostViewModel { get; set; }

        public IEnumerable<CategoryViewModel> CategoryViewModel { get; set; }
    }
}
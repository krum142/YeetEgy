using System.Collections.Generic;

using Yeetegy.Data.Models;
using Yeetegy.Services.Mapping;

namespace Yeetegy.Web.ViewModels.PostViewModels
{
    public class PostDetailsViewModel : IMapFrom<Post>
    {
        public PostCommentsViewModel PostViewModel { get; set; }

        public IEnumerable<CategoryViewModel> CategoryViewModel { get; set; }
    }
}

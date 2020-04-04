using System.Collections.Generic;

namespace Yeetegy.Web.ViewModels.PostViewModels
{
    public class PostDetailsViewModel
    {
        public PostsViewModel PostViewModel { get; set; }

        public IEnumerable<CategoryViewModel> CategoryViewModel { get; set; }
    }
}

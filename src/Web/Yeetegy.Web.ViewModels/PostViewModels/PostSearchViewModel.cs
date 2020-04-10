using System.Collections.Generic;

namespace Yeetegy.Web.ViewModels.PostViewModels
{
    public class PostSearchViewModel
    {
        public IEnumerable<PostsViewModel> PostViewModel { get; set; }

        public IEnumerable<CategoryViewModel> CategoryViewModel { get; set; }
    }
}
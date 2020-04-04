using System.Collections.Generic;

namespace Yeetegy.Web.ViewModels.PostViewModels
{
    public class UserPostsViewModel
    {
        public IEnumerable<PostsViewModel> Posts { get; set; }

        public IEnumerable<CategoryViewModel> Categorys { get; set; }
    }
}

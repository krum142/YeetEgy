using System.Collections.Generic;
using System.Threading.Tasks;
using Yeetegy.Web.ViewModels;
using Yeetegy.Web.ViewModels.PostViewModels;

namespace Yeetegy.Services.Data.Interfaces
{
    public interface IPostsService
    {
        Task CreatePostAsync(AddPostsViewModel post, string userId, string cloudSettings);

        IEnumerable<PostsViewModel> GetFivePosts(int skip);
    }
}
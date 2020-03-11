using System.Collections.Generic;
using System.Threading.Tasks;
using Yeetegy.Web.ViewModels;
using Yeetegy.Web.ViewModels.PostViewModels;

namespace Yeetegy.Services.Data.Interfaces
{
    public interface IPostsService
    {
        Task CreatePostAsync(AddPostsViewModel post, string userId);

        IEnumerable<T> GetPosts<T>(int skip, int take, string category = null);

        IEnumerable<T> GetPostsPopular<T>(int skip, int take);

        IEnumerable<T> GetPostsTrending<T>(int skip, int take);

        Task LikePostAsync(string id);
    }
}
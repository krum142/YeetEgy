using System.Collections.Generic;
using System.Threading.Tasks;

using Yeetegy.Web.ViewModels;
using Yeetegy.Web.ViewModels.PostViewModels;

namespace Yeetegy.Services.Data.Interfaces
{
    public interface IPostsService
    {
        Task CreatePostAsync(AddPostsModel post, string userId);

        Task<IEnumerable<T>> GetPostsAsync<T>(int skip, int take, string category = null);

        Task<IEnumerable<T>> GetPostsPopularAsync<T>(int skip, int take);

        Task<IEnumerable<T>> GetPostsTrendingAsync<T>(int skip, int take);

        Task<string> GetPostVoteValueAsync(string postId, string userId);

        Task<bool> DoesPostExistAsync(string postId);

        Task<T> GetPostAsync<T>(string postId);

        Task<IEnumerable<T>> GetUserLikedAsync<T>(int skip, int take, string username);

        Task<IEnumerable<T>> GetUserCommentedAsync<T>(int skip, int take, string userId);

        Task<IEnumerable<T>> GetUserPostsAsync<T>(int skip, int take, string userId);

        Task<IEnumerable<T>> GetAllByTagAsync<T>(int skip, int take, string tag);
    }
}

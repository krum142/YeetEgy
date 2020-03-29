using System.Collections.Generic;
using System.Threading.Tasks;

using Yeetegy.Web.ViewModels;

namespace Yeetegy.Services.Data.Interfaces
{
    public interface ICommentsService
    {
        Task CreateCommentAsync(AddCommentsModel input, string userId);

        Task<bool> DoesCommentExistAsync(string commentId);

        Task<string> CommentVoteAsync(string commentId, string userId, bool isUpVote);

        Task<IEnumerable<T>> GetCommentsAsync<T>(string postId, int skip, int take);
    }
}
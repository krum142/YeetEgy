using System.Threading.Tasks;

namespace Yeetegy.Services.Data.Interfaces
{
    public interface IVotesService
    {
        Task<string> VoteAsync(string postId, string userId, bool isUpVote);
    }
}
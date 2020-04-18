using System.Collections.Generic;
using System.Threading.Tasks;
using Yeetegy.Web.ViewModels;
using Yeetegy.Web.ViewModels.ReplayModels;

namespace Yeetegy.Services.Data.Interfaces
{
    public interface IReplaysService
    {
        Task CreateReplayAsync(AddReplayModel input, string userId);

        Task<IEnumerable<T>> AllAsync<T>(string commentId);
    }
}

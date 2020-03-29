using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yeetegy.Services.Data.Interfaces
{
    public interface IReplaysService
    {
        Task<IEnumerable<T>> AllAsync<T>(string commentId);
    }
}

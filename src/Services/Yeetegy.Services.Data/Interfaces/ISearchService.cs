using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yeetegy.Services.Data.Interfaces
{
    public interface ISearchService
    {
        Task<IEnumerable<string>> GetSearchResultsAsync(string queryToSearch);
    }
}
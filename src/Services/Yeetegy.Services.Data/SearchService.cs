using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Yeetegy.Data.Common.Repositories;
using Yeetegy.Data.Models;
using Yeetegy.Services.Data.Interfaces;

namespace Yeetegy.Services.Data
{
    public class SearchService : ISearchService
    {
        private readonly IDeletableEntityRepository<Post> postsRepository;

        public SearchService(IDeletableEntityRepository<Post> postsRepository)
        {
            this.postsRepository = postsRepository;
        }

        public async Task<IEnumerable<string>> GetSearchResultsAsync(string queryToSearch)
        {
            return await this.postsRepository.AllAsNoTracking().Where(x => x.Tittle.Contains(queryToSearch)).Select(x => x.Tittle).Distinct()
                .ToListAsync();
        }
    }
}

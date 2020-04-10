using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Yeetegy.Data.Common.Repositories;
using Yeetegy.Data.Models;
using Yeetegy.Services.Data.Interfaces;
using Yeetegy.Services.Mapping;

namespace Yeetegy.Services.Data
{
    public class SearchService : ISearchService
    {
        private readonly IDeletableEntityRepository<Tag> tagRepository;

        public SearchService(IDeletableEntityRepository<Tag> tagRepository)
        {
            this.tagRepository = tagRepository;
        }

        public async Task<IEnumerable<T>> GetSearchResultsAsync<T>(string queryToSearch)
        {
            return await this.tagRepository
                .AllAsNoTracking()
                .Where(x => x.Value.Contains(queryToSearch))
                .To<T>()
                .ToListAsync();
        }
    }
}

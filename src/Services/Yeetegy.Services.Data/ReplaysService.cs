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
    public class ReplaysService : IReplaysService
    {
        private readonly IDeletableEntityRepository<Comment> replayRepository;

        public ReplaysService(IDeletableEntityRepository<Comment> replayRepository)
        {
            this.replayRepository = replayRepository;
        }

        public async Task<IEnumerable<T>> AllAsync<T>(string commentId)
        {
           return await this.replayRepository
               .AllAsNoTracking()
               .Where(r => r.ReplayId == commentId)
               .OrderByDescending(r => r.Likes)
               .To<T>()
               .ToListAsync();
        }
    }
}

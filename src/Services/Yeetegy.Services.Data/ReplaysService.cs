using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Yeetegy.Data.Common.Repositories;
using Yeetegy.Data.Models;
using Yeetegy.Services.Data.Interfaces;
using Yeetegy.Services.Mapping;
using Yeetegy.Web.ViewModels;

namespace Yeetegy.Services.Data
{
    public class ReplaysService : IReplaysService
    {
        private readonly IDeletableEntityRepository<Comment> replayRepository;
        private readonly ICloudinaryService cloudinaryService;

        public ReplaysService(
            IDeletableEntityRepository<Comment> replayRepository,
            ICloudinaryService cloudinaryService)
        {
            this.replayRepository = replayRepository;
            this.cloudinaryService = cloudinaryService;
        }

        public async Task CreateCommentAsync(AddReplayModel input, string userId)
        {
            string url = null;

            if (input.File != null)
            {
                url = await this.cloudinaryService.SaveCloudinaryAsync(input.File);
            }

            var comment = new Comment()
            {
                Description = input.Description,
                ReplayId = input.CommentId,
                ApplicationUserId = userId,
                PostId = input.PostId,
                ImgUrl = url,
            };

            await this.replayRepository.AddAsync(comment);
            await this.replayRepository.SaveChangesAsync();
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

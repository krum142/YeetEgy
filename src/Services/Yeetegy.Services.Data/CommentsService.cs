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
    public class CommentsService : ICommentsService
    {
        private readonly IDeletableEntityRepository<Comment> commentsRepository;
        private readonly ICloudinaryService cloudinaryService;

        public CommentsService(IDeletableEntityRepository<Comment> commentsRepository, ICloudinaryService cloudinaryService)
        {
            this.commentsRepository = commentsRepository;
            this.cloudinaryService = cloudinaryService;
        }

        public async Task CreateCommentAsync(AddCommentsModel input, string userId)
        {
            string url = null;

            if (input.File != null)
            {
                url = await this.cloudinaryService.SaveCloudinaryAsync(input.File);
            }

            var comment = new Comment()
            {
                Description = input.Description,
                ApplicationUserId = userId,
                PostId = input.PostId,
                ImgUrl = url,
            };

            await this.commentsRepository.AddAsync(comment);
            await this.commentsRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetCommentsAsync<T>(string postId, int skip, int take)
        {
            var query = this.commentsRepository.AllAsNoTracking();

            return await query.Where(x => x.PostId == postId).OrderByDescending(x => x.CreatedOn).Skip(skip).Take(take).To<T>().ToListAsync();
        }
    }
}

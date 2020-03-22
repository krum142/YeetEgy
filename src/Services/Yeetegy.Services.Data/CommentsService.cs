using System.Threading.Tasks;

using Yeetegy.Data.Common.Repositories;
using Yeetegy.Data.Models;
using Yeetegy.Services.Data.Interfaces;
using Yeetegy.Web.ViewModels;

namespace Yeetegy.Services.Data
{
    public class CommentsService : ICommentsService
    {
        private readonly IDeletableEntityRepository<Comment> commentsRepository;
        private readonly ICloudinaryService cloudinaryService;

        public CommentsService(IDeletableEntityRepository<Comment> commentsRepository, CloudinaryService cloudinaryService)
        {
            this.commentsRepository = commentsRepository;
            this.cloudinaryService = cloudinaryService;
        }

        public async Task<string> CreateCommentAsync(AddCommentsModel input, string userId)
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
            return "200";
        }
    }
}

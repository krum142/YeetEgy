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
        private readonly IDeletableEntityRepository<UserCommentVote> userCommentVoteRepository;
        private readonly ICloudinaryService cloudinaryService;

        public CommentsService(
            IDeletableEntityRepository<Comment> commentsRepository,
            IDeletableEntityRepository<UserCommentVote> userCommentVoteRepository,
            ICloudinaryService cloudinaryService)
        {
            this.commentsRepository = commentsRepository;
            this.userCommentVoteRepository = userCommentVoteRepository;
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

        public async Task<string> CommentVoteAsync(string commentId, string userId, bool isUpVote)
        {
            var lastVote = await this.userCommentVoteRepository.All()
                .Where(x => x.CommentId == commentId && x.ApplicationUserId == userId)
                .Select(x => x.Value).FirstOrDefaultAsync();

            var comment = await this.commentsRepository.All().FirstOrDefaultAsync(x => x.Id == commentId);
            var status = string.Empty;
            if (lastVote != null)
            {
                var postValue = await this.userCommentVoteRepository
                    .All()
                    .FirstOrDefaultAsync(x => x.CommentId == commentId && x.ApplicationUserId == userId);
                if (isUpVote)
                {
                    if (lastVote == "Like")
                    {
                        this.userCommentVoteRepository.HardDelete(postValue);
                        comment.Likes--;
                        status = "UnLike";
                    }
                    else
                    {
                        postValue.Value = "Like";
                        comment.Likes++;
                        comment.Dislikes--;
                        status = "DislikeToLike";
                    }
                }
                else
                {
                    if (lastVote == "Dislike")
                    {
                        this.userCommentVoteRepository.HardDelete(postValue);
                        comment.Dislikes--;
                        status = "UnDislike";
                    }
                    else
                    {
                        postValue.Value = "Dislike";
                        comment.Likes--;
                        comment.Dislikes++;
                        status = "LikeToDislike";
                    }
                }
            }
            else
            {
                if (isUpVote)
                {
                    await this.userCommentVoteRepository.AddAsync(new UserCommentVote()
                    { CommentId = commentId, ApplicationUserId = userId, Value = "Like" });
                    comment.Likes++;
                    status = "Like";
                }
                else
                {
                    await this.userCommentVoteRepository.AddAsync(new UserCommentVote()
                    { CommentId = commentId, ApplicationUserId = userId, Value = "Dislike" });
                    comment.Dislikes++;
                    status = "Dislike";
                }
            }

            await this.userCommentVoteRepository.SaveChangesAsync();
            await this.commentsRepository.SaveChangesAsync();
            return status;
        }

        public async Task<bool> DoesCommentExistAsync(string commentId)
        {
            return await this.commentsRepository.AllAsNoTracking().AnyAsync(x => x.Id == commentId);
        }

        public async Task<IEnumerable<T>> GetCommentsAsync<T>(string postId, int skip, int take)
        {
            var query = this.commentsRepository.AllAsNoTracking();

            return await query.Where(x => x.PostId == postId && string.IsNullOrWhiteSpace(x.ReplayId)).OrderByDescending(x => x.CreatedOn).Skip(skip).Take(take).To<T>().ToListAsync();
        }
    }
}

using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Yeetegy.Data.Common.Repositories;
using Yeetegy.Data.Models;
using Yeetegy.Services.Data.Interfaces;

namespace Yeetegy.Services.Data
{
    public class VotesService : IVotesService
    {
        private readonly IDeletableEntityRepository<UserPostVote> postVoteRepository;
        private readonly IDeletableEntityRepository<Post> postRepository;

        public VotesService(IDeletableEntityRepository<UserPostVote> postVoteRepository, IDeletableEntityRepository<Post> postRepository)
        {
            this.postVoteRepository = postVoteRepository;
            this.postRepository = postRepository;
        }

        /// <summary>
        /// votes for the specified post and corresponding user
        /// isUpVote == False = downVote
        /// isUpVote == true = up vote.
        /// Possible return types:
        /// Like,Dislike,
        /// UnLike,UnDislike,
        /// LikeToDislike,DislikeToLike.
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="userId"></param>
        /// <param name="isUpVote"></param>
        /// <returns></returns>
        public async Task<string> VoteAsync(string postId, string userId, bool isUpVote)
        {
            var lastVote = await this.postVoteRepository.All()
                .Where(x => x.PostId == postId && x.ApplicationUserId == userId)
                .Select(x => x.Value).FirstOrDefaultAsync();
            var post = await this.postRepository.All().FirstOrDefaultAsync(x => x.Id == postId);
            var status = string.Empty;
            if (lastVote != null)
            {
                var postValue = await this.postVoteRepository
                    .All()
                    .FirstOrDefaultAsync(x => x.PostId == postId && x.ApplicationUserId == userId);
                if (isUpVote)
                {
                    if (lastVote == "Like")
                    {
                        this.postVoteRepository.HardDelete(postValue);
                        post.Likes--;
                        status = "UnLike";
                    }
                    else
                    {
                        postValue.Value = "Like";
                        post.Likes++;
                        post.Dislikes--;
                        status = "DislikeToLike";
                    }
                }
                else
                {
                    if (lastVote == "Dislike")
                    {
                        this.postVoteRepository.HardDelete(postValue);
                        post.Dislikes--;
                        status = "UnDislike";
                    }
                    else
                    {
                        postValue.Value = "Dislike";
                        post.Likes--;
                        post.Dislikes++;
                        status = "LikeToDislike";
                    }
                }
            }
            else
            {
                if (isUpVote)
                {
                    await this.postVoteRepository.AddAsync(new UserPostVote()
                    { PostId = postId, ApplicationUserId = userId, Value = "Like" });
                    post.Likes++;
                    status = "Like";
                }
                else
                {
                    await this.postVoteRepository.AddAsync(new UserPostVote()
                    { PostId = postId, ApplicationUserId = userId, Value = "Dislike" });
                    post.Dislikes++;
                    status = "Dislike";
                }
            }

            await this.postVoteRepository.SaveChangesAsync();
            await this.postRepository.SaveChangesAsync();
            return status;
        }
    }
}

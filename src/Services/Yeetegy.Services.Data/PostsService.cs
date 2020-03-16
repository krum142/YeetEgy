using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Yeetegy.Data.Common.Repositories;
using Yeetegy.Data.Models;
using Yeetegy.Services.Data.Interfaces;
using Yeetegy.Services.Mapping;
using Yeetegy.Web.ViewModels.PostViewModels;

namespace Yeetegy.Services.Data
{
    public class PostsService : IPostsService
    {
        private readonly IDeletableEntityRepository<Post> postRepository;
        private readonly IDeletableEntityRepository<UserPostVote> postVoteRepository;
        private readonly ICategoryService categoryService;
        private readonly ICloudinaryService cloudinary;

        public PostsService(
            IDeletableEntityRepository<Post> postRepository,
            IDeletableEntityRepository<UserPostVote> postVoteRepository,
            ICategoryService categoryService,
            ICloudinaryService cloudinary)
        {
            this.postRepository = postRepository;
            this.postVoteRepository = postVoteRepository;
            this.categoryService = categoryService;
            this.cloudinary = cloudinary;
        }

        public async Task CreatePostAsync(AddPostsViewModel post, string userId)
        {
            var urlTest = this.cloudinary.SaveCloudinary(post.File);

            var url = urlTest.Result;

            if (url != null)
            {
                var newPost = new Post()
                {
                    ApplicationUserId = userId,
                    Tittle = post.Tittle,
                    ImgUrl = url,
                    CategoryId = await this.categoryService.GetIdAsync(post.Category),
                };

                await this.postRepository.AddAsync(newPost);
                await this.postRepository.SaveChangesAsync();
            }
        }

        public async Task LikeAsync(string postId, string userId)
        {
            await this.postVoteRepository.AddAsync(new UserPostVote()
            { PostId = postId, ApplicationUserId = userId, Value = "Like" });

            var post = await this.postRepository.All().FirstOrDefaultAsync(x => x.Id == postId);
            post.Likes++;

            await this.postVoteRepository.SaveChangesAsync();
            await this.postRepository.SaveChangesAsync();
        }

        public async Task UndoLikeAsync(string postId, string userId)
        {
            var postValue = await this.postVoteRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.PostId == postId && x.ApplicationUserId == userId);

            this.postVoteRepository.HardDelete(postValue);

            var post = await this.postRepository.All().FirstOrDefaultAsync(x => x.Id == postId);
            post.Likes--;

            await this.postVoteRepository.SaveChangesAsync();
            await this.postRepository.SaveChangesAsync();
        }

        public async Task DislikeAsync(string postId, string userId)
        {
            await this.postVoteRepository.AddAsync(new UserPostVote()
            { PostId = postId, ApplicationUserId = userId, Value = "Dislike" });

            var post = await this.postRepository.All().FirstOrDefaultAsync(x => x.Id == postId);
            post.Dislikes++;

            await this.postVoteRepository.SaveChangesAsync();
            await this.postRepository.SaveChangesAsync();
        }

        public async Task UndoDislikeAsync(string postId, string userId)
        {
            var postValue = await this.postVoteRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.PostId == postId && x.ApplicationUserId == userId);

            this.postVoteRepository.HardDelete(postValue);

            var post = await this.postRepository.All().FirstOrDefaultAsync(x => x.Id == postId);
            post.Dislikes--;

            await this.postVoteRepository.SaveChangesAsync();
            await this.postRepository.SaveChangesAsync();
        }

        public async Task LikeToDislikeAsync(string postId, string userId)
        {
            var postvalue = await this.postVoteRepository
                .All()
                .FirstOrDefaultAsync(x => x.PostId == postId && x.ApplicationUserId == userId);

            postvalue.Value = "Dislike";

            var post = await this.postRepository.All().FirstOrDefaultAsync(x => x.Id == postId);
            post.Likes--;
            post.Dislikes++;

            await this.postVoteRepository.SaveChangesAsync();
            await this.postRepository.SaveChangesAsync();
        }

        public async Task DislikeToLikeAsync(string postId, string userId)
        {
            var postvalue = await this.postVoteRepository
                .All()
                .FirstOrDefaultAsync(x => x.PostId == postId && x.ApplicationUserId == userId);

            postvalue.Value = "Like";

            var post = await this.postRepository.All().FirstOrDefaultAsync(x => x.Id == postId);
            post.Likes++;
            post.Dislikes--;

            await this.postVoteRepository.SaveChangesAsync();
            await this.postRepository.SaveChangesAsync();
        }

        public async Task<string> GetPostVoteValueAsync(string postId, string userId)
        {
            if (await this.postVoteRepository.AllAsNoTracking()
                .AnyAsync(x => x.PostId == postId && x.ApplicationUserId == userId))
            {
                return await this.postVoteRepository.AllAsNoTracking()
                    .Where(x => x.PostId == postId && x.ApplicationUserId == userId).Select(x => x.Value).FirstOrDefaultAsync();
            }

            return null;
        }

        public async Task<bool> DoesPostExistAsync(string postId)
        {
            return await this.postRepository.AllAsNoTracking().AnyAsync(x => x.Id == postId);
        }

        public async Task<T> GetPostAsync<T>(string postId)
        {
            return await this.postRepository.AllAsNoTracking().Where(x => x.Id == postId).To<T>().FirstOrDefaultAsync();
        }

        // you can use enums to make ifs with categorys (down there)
        public async Task<IEnumerable<T>> GetPostsAsync<T>(int skip, int take, string category = null)
        {
            var query = this.postRepository.AllAsNoTracking();

            if (category != null)
            {
                query = query.Where(x => x.Category.Name == category);
            }

            return await query.OrderByDescending(x => x.CreatedOn).Skip(skip).Take(take).To<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetPostsPopularAsync<T>(int skip, int take)
        {
            var query = this.postRepository.AllAsNoTracking();

            query = query.Where(x => x.Likes >= 200).OrderByDescending(x => x.CreatedOn).ThenByDescending(x => x.CreatedOn);

            return await query.Skip(skip).Take(take).To<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetPostsTrendingAsync<T>(int skip, int take)
        {
            var query = this.postRepository.AllAsNoTracking();

            query = query.OrderByDescending(x => x.Comments.Count).ThenByDescending(x => x.CreatedOn);

            return await query.Skip(skip).Take(take).To<T>().ToListAsync();
        }
    }
}

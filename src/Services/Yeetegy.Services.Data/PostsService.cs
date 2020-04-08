using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        private readonly IDeletableEntityRepository<ApplicationUser> usersRepository;
        private readonly ICategoryService categoryService;
        private readonly ICloudinaryService cloudinary;

        public PostsService(
            IDeletableEntityRepository<Post> postRepository,
            IDeletableEntityRepository<UserPostVote> postVoteRepository,
            IDeletableEntityRepository<ApplicationUser> usersRepository,
            ICategoryService categoryService,
            ICloudinaryService cloudinary)
        {
            this.postRepository = postRepository;
            this.postVoteRepository = postVoteRepository;
            this.usersRepository = usersRepository;
            this.categoryService = categoryService;
            this.cloudinary = cloudinary;
        }

        public async Task CreatePostAsync(AddPostsModel post, string userId)
        {
            var urlTest = this.cloudinary.SaveCloudinaryAsync(post.File);

            var url = urlTest.Result;

            //ICollection<Tag> tags = new List<Tag>();
            //if (!string.IsNullOrWhiteSpace(post.Tags))
            //{
            //    var matches = Regex.Matches(post.Tags, "#[A-Za-z0-9]{1,30}");
            //    foreach (var match in matches)
            //    {
            //        if ()
            //        {

            //        }
            //    }
            //}

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

            query = query.Where(x => x.Likes >= 200).OrderByDescending(x => x.CreatedOn);

            return await query.Skip(skip).Take(take).To<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetPostsTrendingAsync<T>(int skip, int take)
        {
            var query = this.postRepository.AllAsNoTracking();

            query = query.OrderByDescending(x => x.Comments.Count).ThenByDescending(x => x.CreatedOn);

            return await query.Skip(skip).Take(take).To<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetUserLikedAsync<T>(int skip, int take, string userId) // Test me Please
        {
            return await this.postRepository
                .AllAsNoTracking()
                .Where(x => x.UserVotes.Any(y => y.Value == "Like" && y.ApplicationUserId == userId))
                .OrderByDescending(x => x.CreatedOn)
                .Skip(skip).Take(take)
                .To<T>()
                .ToListAsync();
        }

        public async Task<IEnumerable<T>> GetUserCommentedAsync<T>(int skip, int take, string userId)
        {
            return await this.postRepository
                .AllAsNoTracking()
                .Where(x => x.Comments.Any(y => y.ApplicationUserId == userId)).OrderByDescending(x => x.CreatedOn).Skip(skip).Take(take)
                .To<T>()
                .ToListAsync();
        }

        public async Task<IEnumerable<T>> GetUserPostsAsync<T>(int skip, int take, string userId)
        {
            return await this.postRepository
                .AllAsNoTracking()
                .Where(x => x.ApplicationUserId == userId)
                .OrderByDescending(x => x.CreatedOn)
                .Skip(skip).Take(take)
                .To<T>()
                .ToListAsync();
        }
    }
}

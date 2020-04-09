using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Yeetegy.Common;
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
        private readonly IRepository<PostTag> postTagRepository;
        private readonly ICategoryService categoryService;
        private readonly ICloudinaryService cloudinary;
        private readonly ITagService tagService;

        public PostsService(
            IDeletableEntityRepository<Post> postRepository,
            IDeletableEntityRepository<UserPostVote> postVoteRepository,
            IRepository<PostTag> postTagRepository,
            ICategoryService categoryService,
            ICloudinaryService cloudinary,
            ITagService tagService)
        {
            this.postRepository = postRepository;
            this.postVoteRepository = postVoteRepository;
            this.postTagRepository = postTagRepository;
            this.categoryService = categoryService;
            this.cloudinary = cloudinary;
            this.tagService = tagService;
        }

        public async Task CreatePostAsync(AddPostsModel post, string userId)
        {
            var urlTest = this.cloudinary.SaveCloudinaryAsync(post.File);

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

                var tagIds = new List<string>();
                if (!string.IsNullOrWhiteSpace(post.Tags))
                {
                    var matches = Regex.Matches(post.Tags, GlobalConstants.TagValidationRegex);
                    foreach (Match match in matches)
                    {
                        var tag = match.Groups[0].Value.TrimStart('#');
                        if (await this.tagService.ExistsAsync(tag))
                        {
                            tagIds.Add(await this.tagService.GetId(tag));
                        }
                        else
                        {
                            var tagId = await this.tagService.CreateAsync(tag);
                            tagIds.Add(tagId);
                        }
                    }
                }

                var postTags = tagIds.Select(tag => new PostTag() { PostId = newPost.Id, TagId = tag, }).ToList();

                await this.postRepository.AddAsync(newPost);
                await this.postTagRepository.AddRangeAsync(postTags);

                await this.postRepository.SaveChangesAsync();
                await this.postTagRepository.SaveChangesAsync();
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

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
        private readonly ICategoryService categoryService;
        private readonly ICloudinaryService cloudinary;
        private readonly IUserService userService;

        public PostsService(
            IDeletableEntityRepository<Post> postRepository,
            ICategoryService categoryService,
            ICloudinaryService cloudinary,
            IUserService userService)
        {
            this.postRepository = postRepository;
            this.categoryService = categoryService;
            this.cloudinary = cloudinary;
            this.userService = userService;
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

        public async Task LikePostAsync(string postId, string userId)
        {
            var post = await this.postRepository.All().FirstOrDefaultAsync(x => x.Id == postId);

            await this.userService.AddPostAsync(post,userId);
            post.Likes++;

            await this.postRepository.SaveChangesAsync();
        }

        // you can use enums to make ifs with categorys (down there)
        public IEnumerable<T> GetPosts<T>(int skip, int take, string category = null)
        {
            var query = this.postRepository.AllAsNoTracking();

            if (category != null)
            {
                query = query.Where(x => x.Category.Name == category);
            }

            return query.OrderByDescending(x => x.CreatedOn).Skip(skip).Take(take).To<T>().ToList();
        }

        public IEnumerable<T> GetPostsPopular<T>(int skip, int take)
        {
            var query = this.postRepository.AllAsNoTracking();

            query = query.OrderByDescending(x => x.Likes).ThenByDescending(x => x.CreatedOn);

            return query.Skip(skip).Take(take).To<T>().ToList();
        }

        public IEnumerable<T> GetPostsTrending<T>(int skip, int take)
        {
            var query = this.postRepository.AllAsNoTracking();

            query = query.OrderByDescending(x => x.Comments.Count).ThenByDescending(x => x.CreatedOn);

            return query.Skip(skip).Take(take).To<T>().ToList();
        }

    }
}

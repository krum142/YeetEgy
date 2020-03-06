using System.Collections.Generic;
using System.Linq;
using Yeetegy.Web.ViewModels;

namespace Yeetegy.Services.Data
{
    using System.Threading.Tasks;
    using Yeetegy.Data.Common.Repositories;
    using Yeetegy.Data.Models;
    using Interfaces;
    using Web.ViewModels.PostViewModels;

    public class PostsService : IPostsService
    {
        private readonly IDeletableEntityRepository<Post> postRepository;
        private readonly ICategoryService categoryService;
        private readonly ICloudinaryService cloudinary;

        public PostsService(IDeletableEntityRepository<Post> postRepository, ICategoryService categoryService, ICloudinaryService cloudinary)
        {
            this.postRepository = postRepository;
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
                    CategoryId = this.categoryService.GetId(post.Category),
                };

                await this.postRepository.AddAsync(newPost);
                await this.postRepository.SaveChangesAsync();
            }
        }

        public IEnumerable<PostsViewModel> GetFivePostsCategory(int skip, string category)
        {
            return this.postRepository.AllAsNoTracking().Where(x => x.Category.Name == category).OrderByDescending(x => x.CreatedOn).Skip(skip).Take(5).Select(p =>
                new PostsViewModel()
                {
                    Id = p.Id,
                    ImgUrl = p.ImgUrl,
                    Tittle = p.Tittle,
                    CategoryId = p.CategoryId,
                    Dislikes = p.Dislikes,
                    Likes = p.Likes,
                }).ToList();
        }

        public IEnumerable<PostsViewModel> GetFivePostsLatest(int skip)
        {

            return this.postRepository.AllAsNoTracking()
                .OrderByDescending(x => x.CreatedOn).Skip(skip).Take(5).Select(p =>
                new PostsViewModel()
                {
                    Id = p.Id,
                    ImgUrl = p.ImgUrl,
                    Tittle = p.Tittle,
                    CategoryId = p.CategoryId,
                    Dislikes = p.Dislikes,
                    Likes = p.Likes,
                }).ToList();
        }
    }
}

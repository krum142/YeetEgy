namespace Yeetegy.Services.Data
{
    using System;
    using System.Threading.Tasks;

    using CloudinaryDotNet;
    using Microsoft.AspNetCore.Http;
    using Yeetegy.Data.Common.Repositories;
    using Yeetegy.Data.Models;
    using CommonServices;
    using Interfaces;
    using Web.ViewModels.PostViewModels;

    public class PostsService : IPostsService
    {
        private readonly IDeletableEntityRepository<Post> postRepository;
        private readonly ICategoryService categoryService;

        public PostsService(IDeletableEntityRepository<Post> postRepository, ICategoryService categoryService)
        {
            this.postRepository = postRepository;
            this.categoryService = categoryService;
        }

        public async Task CreatePostAsync(AddPostsViewModel post, string userId, string cloudSettings)
        {
            var urlTest = SaveCloudinary(post.File, cloudSettings);

            var url = urlTest.Result;

            if (url != null)
            {
                var newPost = new Post()
                {
                    ApplicationUserId = userId,
                    Tittle = post.Tittle,
                    ImgUrl = url,
                    CategoryId = categoryService.GetId(post.Category),
                };

                await this.postRepository.AddAsync(newPost);
                await this.postRepository.SaveChangesAsync();
            }
        }

        private async Task<string> SaveCloudinary(IFormFile image, string cloudSettings)
        {
            var settings = cloudSettings.Split("$");
            var cloudName = settings[0];
            var apikey = settings[1];
            var apiSec = settings[2];
            var fileName = Guid.NewGuid().ToString();

            var account = new Account(cloudName, apikey, apiSec);
            var cloud = new Cloudinary(account);

            return await ApplicationCloudinary.UploadImage(cloud, image, fileName);
        }
    }
}

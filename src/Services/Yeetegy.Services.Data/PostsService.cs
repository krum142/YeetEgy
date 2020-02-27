﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Yeetegy.Web.ViewModels;

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
        private readonly IConfiguration configuration;

        public PostsService(IDeletableEntityRepository<Post> postRepository, ICategoryService categoryService, IConfiguration configuration)
        {
            this.postRepository = postRepository;
            this.categoryService = categoryService;
            this.configuration = configuration;
        }

        public async Task CreatePostAsync(AddPostsViewModel post, string userId)
        {
            var urlTest = SaveCloudinary(post.File);

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

        public IEnumerable<PostsViewModel> GetFivePosts(int skip)
        {
            var posts = this.postRepository.All().OrderByDescending(x => x.CreatedOn).Skip(skip).Take(5).Select(p =>
                new PostsViewModel()
                {
                    Id = p.Id,
                    ImgUrl = p.ImgUrl,
                    Tittle = p.Tittle,
                    CategoryId = p.CategoryId,
                    Dislikes = p.Dislikes,
                    Likes = p.Likes,
                }).ToList();

            return posts;
        }

        private async Task<string> SaveCloudinary(IFormFile image)
        {
            var settings = this.configuration["CloudSettings"].Split("$");
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

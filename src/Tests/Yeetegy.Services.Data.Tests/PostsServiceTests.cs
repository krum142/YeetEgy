using System;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Yeetegy.Data;
using Yeetegy.Data.Models;
using Yeetegy.Data.Repositories;
using Yeetegy.Web.ViewModels.Administration.Dashboard;
using Yeetegy.Web.ViewModels.PostViewModels;

namespace Yeetegy.Services.Data.Tests
{
    public class PostsServiceTests : IDisposable
    {
        private readonly PostsService postsService;
        private readonly ApplicationDbContext dbContext;

        public PostsServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "PostsTestDb").Options;

            this.dbContext = new ApplicationDbContext(options);
            var categoryRepository = new EfDeletableEntityRepository<Category>(this.dbContext);
            var postRepository = new EfDeletableEntityRepository<Post>(this.dbContext);
            var userPostVoteRepository = new EfDeletableEntityRepository<UserPostVote>(this.dbContext);
            var postTagRepository = new EfRepository<PostTag>(this.dbContext);
            var tagRepository = new EfDeletableEntityRepository<Tag>(this.dbContext);

            var tagService = new TagService(tagRepository);

            var cloudinaryService = new FakeCloudinary();

            var categoryService = new CategoryService(categoryRepository, cloudinaryService);

            this.postsService = new PostsService(postRepository, userPostVoteRepository, postTagRepository,
                categoryService, cloudinaryService, tagService);

            this.dbContext.Database.EnsureCreated();

            var user1 = new ApplicationUser()
            {
                Id = "1",
                UserName = "Pesho",
            };

            this.dbContext.Users.Add(user1);
            this.dbContext.SaveChanges();
        }

        //[Fact]
        //public async Task CreatePostWithValidInputTest()
        //{
        //    var addpostModel = new AddPostsModel();
        //    addpostModel.Category


        //    this.postsService.CreatePostAsync(,"1")



        //}

        public void Dispose()
        {
            this.dbContext.Database.EnsureDeleted();
        }
    }
}
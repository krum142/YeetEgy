using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Yeetegy.Data;
using Yeetegy.Data.Models;
using Yeetegy.Data.Repositories;
using Yeetegy.Services.Data.Tests.TestModels;
using Yeetegy.Services.Mapping;
using Yeetegy.Web.ViewModels;
using Yeetegy.Web.ViewModels.PostViewModels;

namespace Yeetegy.Services.Data.Tests
{
    public class PostsServiceTests : IDisposable
    {
        private PostsService postsService;
        private ApplicationDbContext dbContext;

        [SetUp]
        public async Task SetUp()
        {
            AutoMapperConfig.RegisterMappings(typeof(TestPostClass).Assembly);

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

            this.postsService = new PostsService(
                postRepository,
                userPostVoteRepository,
                postTagRepository,
                categoryService,
                cloudinaryService,
                tagService);

            this.dbContext.Database.EnsureCreated();

            var user1 = new ApplicationUser()
            {
                Id = "userId1",
                UserName = "Pesho",
            };

            var user2 = new ApplicationUser()
            {
                Id = "userId2",
                UserName = "Ivan",
            };

            await this.dbContext.Users.AddAsync(user1);
            await this.dbContext.Users.AddAsync(user2);
            await this.dbContext.SaveChangesAsync();
        }

        [Test]
        public async Task CreatePostWithValidInputTest()
        {
            await this.AddTwoCategorysAsync();

            IFormFile file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", "dummy.png");

            var postViewModel = new AddPostsModel()
            {
                File = file,
                Tittle = "Pesho",
                Category = "Funny",
            };

            await this.postsService.CreatePostAsync(postViewModel, "userId1");

            var actual = await this.dbContext.Posts.FirstOrDefaultAsync(x => x.Tittle == "Pesho");
            var expectedTittle = "Pesho";
            var expectedCategory = "Funny";
            var expectedUrl = "FakeCloudinaryUrl";

            Assert.IsTrue(expectedTittle == actual.Tittle && expectedCategory == actual.Category.Name && expectedUrl == actual.ImgUrl);
        }

        [Test]
        public async Task CreatePostWithTagsTest()
        {
            await this.AddTwoCategorysAsync();

            IFormFile file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", "dummy.png");

            var postViewModel = new AddPostsModel()
            {
                File = file,
                Tittle = "Pesho",
                Category = "Funny",
                Tags = "#SoFunny Funny",
            };

            await this.postsService.CreatePostAsync(postViewModel, "userId1");

            var actual = await this.dbContext.Posts.FirstOrDefaultAsync(x => x.Tittle == "Pesho");
            var expectedTittle = "Pesho";
            var expectedCategory = "Funny";
            var expectedUrl = "FakeCloudinaryUrl";
            var expectedTag = "SoFunny";

            Assert.IsTrue(expectedTittle == actual.Tittle &&
                          expectedCategory == actual.Category.Name &&
                          expectedUrl == actual.ImgUrl &&
                          actual.PostTags.Any(x => x.Tag.Value == expectedTag));
        }

        [Test]
        public async Task TestCreatePostsWithSameTags()
        {
            await this.AddTwoCategorysAsync();

            IFormFile file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", "dummy.png");

            var postViewModel = new AddPostsModel()
            {
                File = file,
                Tittle = "Pesho",
                Category = "Funny",
                Tags = "#SoFunny Funny",
            };

            await this.postsService.CreatePostAsync(postViewModel, "userId1");

            await this.postsService.CreatePostAsync(postViewModel, "userId1");

            Assert.IsTrue(this.dbContext.Tags.Count(x => x.Value == "SoFunny") == 1);
        }

        [Test]
        public async Task TestTakePostAuthorIdWithValidAuthor()
        {
            await this.AddTwoPostsAsync();

            var actual = await this.postsService.TakeAuthorIdAsync("postId1");
            var expected = "userId1";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task TestTakePostAuthorWithInvalidPost()
        {
            await this.AddTwoPostsAsync();

            var actual = await this.postsService.TakeAuthorIdAsync("InvalidPost");

            Assert.AreEqual(null, actual);
        }

        [Test]
        public async Task TestDeletePostWithExistingPost()
        {
            await this.AddTwoPostsAsync();

            await this.postsService.DeletePostAsync("postId1");

            var actual = this.dbContext.Posts.Count();
            var expected = 1;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task TestDeleteTwoPostsWithExistingPosts()
        {
            await this.AddTwoPostsAsync();

            await this.postsService.DeletePostAsync("postId1");
            await this.postsService.DeletePostAsync("postId2");

            var actual = this.dbContext.Posts.Count();
            var expected = 0;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task TestDoesPostExistWithExistingPost()
        {
            await this.AddTwoPostsAsync();

            var actual = await this.postsService.DoesPostExistAsync("postId1");

            Assert.IsTrue(actual);
        }

        [Test]
        public async Task TestDoesPostExistWithNoneExistingPost()
        {
            var actual = await this.postsService.DoesPostExistAsync("postId1");

            Assert.IsFalse(actual);
        }

        //[Test]
        //public async Task TestGetPostWithExistingPost() // does not work with PostsViewModel
        //{
        //    await this.AddTwoPostsAsync();

        //    var expected = await this.postsService.GetPostAsync<TestPostClass>("postId1");

        //    var x = 5;

        //}

        [TearDown]
        public void Dispose()
        {
            this.dbContext.Database.EnsureDeleted();
        }

        private async Task AddTwoCategorysAsync()
        {
            var cateogrys = new List<Category>()
            {
                new Category()
                {
                    Id = "1",
                    Name = "Funny",
                    ImageUrl = "Url1",
                },
                new Category()
                {
                    Id = "2",
                    Name = "Cool",
                    ImageUrl = "Url2",
                },
            };

            await this.dbContext.Categories.AddRangeAsync(cateogrys);
            await this.dbContext.SaveChangesAsync();
        }

        private async Task AddTwoPostsAsync()
        {
            await this.AddTwoCategorysAsync();

            var posts = new List<Post>()
            {
                new Post()
                {
                    Id = "postId1",
                    ApplicationUserId = "userId1",
                    Tittle = "Tittle1",
                    ImgUrl = "Url1",
                    CategoryId = "1",
                },
                new Post()
                {
                    Id = "postId2",
                    ApplicationUserId = "userId2",
                    Tittle = "Tittle2",
                    ImgUrl = "Url2",
                    CategoryId = "2",
                },
            };

            await this.dbContext.Posts.AddRangeAsync(posts);
            await this.dbContext.SaveChangesAsync();
        }
    }
}
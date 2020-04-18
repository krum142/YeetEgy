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
using Yeetegy.Web.ViewModels.PostViewModels;

namespace Yeetegy.Services.Data.Tests
{
    public class PostsServiceTests : BaseServiceTests
    {
        private PostsService postsService;
        private ApplicationDbContext dbContext;

        [SetUp]
        public async Task SetUp()
        {
            //AutoMapperConfig.RegisterMappings(typeof(TestPostClass).Assembly);


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

            await this.dbContext.Database.EnsureCreatedAsync();

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
        public async Task TestGetPostWithExistingPost()
        {
            await this.AddTwoPostsAsync();

            var actual = await this.postsService.GetPostAsync<TestPostClass>("postId1");

            var expectedTittle = "Tittle1";
            var expectedCategory = "Funny";
            var expectedUrl = "Url1";

            Assert.IsTrue(actual.Tittle == expectedTittle &&
                          actual.CategoryName == expectedCategory &&
                          actual.CategoryImageUrl == expectedUrl);
        }

        [Test]
        public async Task TestGetPostWithNoneExistingPost()
        {
            var actual = await this.postsService.GetPostAsync<TestPostClass>("postId1");

            Assert.IsNull(actual);
        }

        [Test]
        public async Task TestGetPostsWithExistingPosts()
        {
            await this.AddTwoPostsAsync();

            var actual = await this.postsService
                .GetPostsAsync<TestPostClass>(0, 5);

            var expected = 2;

            Assert.IsTrue(actual.Count() == expected);
        }

        [Test]
        public async Task TestGetPostsWithCategoryAndExistingPosts()
        {
            await this.AddTwoPostsAsync();

            var actual = await this.postsService
                .GetPostsAsync<TestPostClass>(0, 5, "Funny");

            var expected = 1;

            Assert.IsTrue(actual.Count() == expected);
        }

        [Test]
        public async Task TestGetPostsWithNoPosts()
        {
            var actual = await this.postsService
                .GetPostsAsync<TestPostClass>(0, 5);
            var expected = 0;
            Assert.AreEqual(expected, actual.Count());
        }

        [Test]
        public async Task TestGetPostsPopularWithExistingPosts()
        {
            await this.AddTwoPostsAsync();

            var actual = await this.postsService
                .GetPostsPopularAsync<TestPostClass>(0, 5);

            var expected = 1;

            Assert.IsTrue(actual.Count() == expected);
        }

        [Test]
        public async Task TestGetPostsPopularWithNoPosts()
        {
            var actual = await this.postsService
                .GetPostsPopularAsync<TestPostClass>(0, 5);
            var expected = 0;
            Assert.AreEqual(expected, actual.Count());
        }

        [Test]
        public async Task TestGetPostsTrendingWithExistingPosts()// here we expect

        // that the first post
        // will be the one with the most comments
        {
            await this.AddTwoPostsAsync();
            //await this.AddTwoCommentsToPost("postId2", "userId2");

            var actual = await this.postsService
                .GetPostsTrendingAsync<TestPostClass>(0, 5);

            var expected = 2;
            var expectedFirstPostId = "postId2";

            Assert.IsTrue(actual.Count() == expected && actual.FirstOrDefault().Id == expectedFirstPostId);
        }

        [Test]
        public async Task TestGetPostsTrendingWithNoPosts()
        {
            var actual = await this.postsService
                .GetPostsTrendingAsync<TestPostClass>(0, 5);
            var expected = 0;
            Assert.AreEqual(expected, actual.Count());
        }

        [Test]
        public async Task TestGetUserLikedWith()
        {
            await this.AddTwoPostsAsync();
            await this.UserLikeTwoPosts("userId2", "postId1", "postId2");
            var actual = await this.postsService.GetUserLikedAsync<TestPostClass>(0, 5, "userId2");

            Assert.AreEqual(2, actual.Count());
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

        [TearDown]
        public async Task EndPoint()
        {
            await this.dbContext.Database.EnsureDeletedAsync();
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

        private async Task AddTwoCommentsToPost(string postId, string userId)
        {
            var comments = new List<Comment>()
            {
                new Comment()
                {
                    Id = "commentId1",
                    ApplicationUserId = userId,
                    Description = "Comment1Desc",
                    PostId = postId,
                },
                new Comment()
                {
                    Id = "commentId2",
                    ApplicationUserId = userId,
                    Description = "Comment2Desc",
                    PostId = postId,
                },
            };

            await this.dbContext.Comments.AddRangeAsync(comments);
            await this.dbContext.SaveChangesAsync();
        }

        private async Task UserLikeTwoPosts(string userId, string firstPostId, string secondPostId)
        {
            var userPostVotes = new List<UserPostVote>()
            {
                new UserPostVote()
                {
                    ApplicationUserId = userId,
                    PostId = firstPostId,
                    Value = "Like",
                },
                new UserPostVote()
                {
                    ApplicationUserId = userId,
                    PostId = secondPostId,
                    Value = "Like",
                },
            };

            await this.dbContext.UserPostVotes.AddRangeAsync(userPostVotes);
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
                    Likes = 25,
                },
                new Post()
                {
                    Id = "postId2",
                    ApplicationUserId = "userId2",
                    Tittle = "Tittle2",
                    ImgUrl = "Url2",
                    CategoryId = "2",
                    Likes = 5,
                },
            };

            await this.AddTwoCommentsToPost("postId2", "userId2");

            await this.dbContext.Posts.AddRangeAsync(posts);
            await this.dbContext.SaveChangesAsync();
        }
    }
}
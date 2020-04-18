using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Yeetegy.Data;
using Yeetegy.Data.Models;
using Yeetegy.Data.Repositories;
using Yeetegy.Services.Data.Interfaces;
using Yeetegy.Web.ViewModels.ReplayModels;

namespace Yeetegy.Services.Data.Tests
{
    public class ReplayServiceTests : BaseServiceTests
    {
        private IReplaysService replayService;
        private ApplicationDbContext dbContext;

        [SetUp]
        public async Task SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "commentsTestDb").Options;
            this.dbContext = new ApplicationDbContext(options);
            await this.dbContext.Database.EnsureDeletedAsync();

            var commentRepository = new EfDeletableEntityRepository<Comment>(this.dbContext);

            this.replayService = new ReplaysService(commentRepository, new FakeCloudinary());
        }

        [Test]
        public async Task TestCreateReplayWithValidInput()
        {
            var userId = "userId1";
            var postId = "postId1";

            var fileName = "Img";
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, fileName, "dummy.png");

            await this.AddTwoPostsAsync();
            await this.AddTwoUsersAsync();
            await this.AddTwoCommentsToPostAsync(postId, userId);

            var model = new AddReplayModel()
            {
                CommentId = "commentId1",
                Description = "TestTextDesc",
                PostId = postId,
                File = file,
            };

            await this.replayService.CreateReplayAsync(model, userId);

            var expectedDescription = "TestTextDesc";
            var expectedUserId = userId;
            var expectedImageUrl = fileName;

            var actual = await this.dbContext.Comments.FirstOrDefaultAsync(x =>
                x.ReplayId == "commentId1" && x.Description == "TestTextDesc" && x.ApplicationUserId == userId);

            Assert.IsTrue(expectedImageUrl == actual.ImgUrl && expectedUserId == userId && expectedDescription == actual.Description);
        }

        [Test]
        public async Task TestCreateReplayWithNullFile()
        {
            var userId = "userId1";
            var postId = "postId1";

            await this.AddTwoPostsAsync();
            await this.AddTwoUsersAsync();
            await this.AddTwoCommentsToPostAsync(postId, userId);

            var model = new AddReplayModel()
            {
                CommentId = "commentId1",
                Description = "TestTextDesc",
                PostId = postId,
                File = null,
            };

            await this.replayService.CreateReplayAsync(model, userId);

            var expectedDescription = "TestTextDesc";
            var expectedUserId = userId;
            string expectedImageUrl = null;

            var actual = await this.dbContext.Comments.FirstOrDefaultAsync(x =>
                x.ReplayId == "commentId1" && x.Description == "TestTextDesc" && x.ApplicationUserId == userId);

            Assert.IsTrue(expectedImageUrl == actual.ImgUrl && expectedUserId == userId && expectedDescription == actual.Description);
        }

        [Test]
        public async Task TestGetAllReplaysWithExistingReplays()
        {
            var userId = "userId1";
            var postId = "postId1";
            var commentId = "commentId1";

            await this.AddTwoPostsAsync();
            await this.AddTwoUsersAsync();
            await this.AddTwoCommentsToPostAsync(postId, userId);
            await this.AddTwoReplaysToCommentAsync(postId, userId, commentId);

            var actual = await this.replayService.AllAsync<ReplayViewModel>(commentId);

            var expected = 2;

            Assert.AreEqual(expected, actual.Count());
        }

        [Test]
        public async Task TestGetAllReplaysWithNoExisting()
        {
            var actual = await this.replayService.AllAsync<ReplayViewModel>("NoneExistantId");

            var expected = 0;

            Assert.AreEqual(expected, actual.Count());
        }

        [TearDown]
        public async Task EndPoint()
        {
            await this.dbContext.Database.EnsureDeletedAsync();
        }

        private async Task AddTwoUsersAsync()
        {
            var users = new List<ApplicationUser>()
            {
                new ApplicationUser()
                {
                    Id = "userId1",
                    UserName = "Pesho",
                },
                new ApplicationUser()
                {
                    Id = "userId2",
                    UserName = "Ivan",
                },
            };

            await this.dbContext.Users.AddRangeAsync(users);
            await this.dbContext.SaveChangesAsync();
        }

        private async Task AddTwoPostsAsync()
        {
            var posts = new List<Post>()
            {
                new Post()
                {
                    Id = "postId1",
                    ApplicationUserId = "userId1",
                    Tittle = "Tittle1",
                    ImgUrl = "Url1",
                    CategoryId = "1",
                    Likes = 0,
                },
                new Post()
                {
                    Id = "postId2",
                    ApplicationUserId = "userId2",
                    Tittle = "Tittle2",
                    ImgUrl = "Url2",
                    CategoryId = "2",
                    Likes = 0,
                },
            };

            await this.dbContext.Posts.AddRangeAsync(posts);
            await this.dbContext.SaveChangesAsync();
        }

        private async Task AddTwoCommentsToPostAsync(string postId, string userId)
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

        private async Task AddTwoReplaysToCommentAsync(string postId, string userId, string commentId)
        {
            var comments = new List<Comment>()
            {
                new Comment()
                {
                    Id = "replayId1",
                    ApplicationUserId = userId,
                    Description = "Replay2Desc",
                    ReplayId = commentId,
                    PostId = postId,
                },
                new Comment()
                {
                    Id = "replayId2",
                    ApplicationUserId = userId,
                    Description = "Replay2Desc",
                    ReplayId = commentId,
                    PostId = postId,
                },
            };

            await this.dbContext.Comments.AddRangeAsync(comments);
            await this.dbContext.SaveChangesAsync();
        }
    }
}
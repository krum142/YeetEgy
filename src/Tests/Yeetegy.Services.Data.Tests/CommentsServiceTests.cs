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
using Yeetegy.Services.Data.Interfaces;
using Yeetegy.Web.ViewModels.CommentModels;

namespace Yeetegy.Services.Data.Tests
{
    public class CommentsServiceTests : BaseServiceTests
    {
        private ICommentsService commentsService;
        private ApplicationDbContext dbContext;

        [SetUp]
        public async Task SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "commentsTestDb").Options;
            this.dbContext = new ApplicationDbContext(options);
            await this.dbContext.Database.EnsureDeletedAsync();

            var commentRepository = new EfDeletableEntityRepository<Comment>(this.dbContext);
            var userCommentVoteRepository = new EfDeletableEntityRepository<UserCommentVote>(this.dbContext);
            this.commentsService = new CommentsService(commentRepository, userCommentVoteRepository, new FakeCloudinary());
        }

        [Test]
        public async Task TestCreateCommentWithValidComment()
        {
            await this.AddTwoUsersAsync();
            await this.AddTwoPostsAsync();

            var fileName = "Img";
            IFormFile file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, fileName, "dummy.png");
            var commentDesc = "TestCommentDescription";
            var commentPostId = "postId1";

            var comentModel = new AddCommentsModel()
            {
                Description = commentDesc,
                PostId = commentPostId,
                File = file,
            };

            await this.commentsService.CreateCommentAsync(comentModel, "userId1");

            var actualComment = await this.dbContext.Comments
                .Where(x => x.PostId == commentPostId && x.Description == commentDesc).FirstOrDefaultAsync();

            var expectedCommentDesc = commentDesc;
            var expectedCommentPostId = commentPostId;
            var expectedCommentUrl = fileName;

            Assert.IsTrue(expectedCommentUrl == actualComment.ImgUrl &&
                          expectedCommentDesc == actualComment.Description &&
                          expectedCommentPostId == actualComment.PostId);
        }

        [Test]
        public async Task TestCreateCommentWithNullFile()
        {
            await this.AddTwoUsersAsync();
            await this.AddTwoPostsAsync();

            var commentDesc = "TestCommentDescription";
            var commentPostId = "postId1";

            var comentModel = new AddCommentsModel()
            {
                Description = commentDesc,
                PostId = commentPostId,
                File = null,
            };

            await this.commentsService.CreateCommentAsync(comentModel, "userId1");

            var actualComment = await this.dbContext.Comments
                .Where(x => x.PostId == commentPostId && x.Description == commentDesc).FirstOrDefaultAsync();

            var expectedCommentDesc = commentDesc;
            var expectedCommentPostId = commentPostId;
            string expectedCommentUrl = null;

            Assert.IsTrue(expectedCommentUrl == actualComment.ImgUrl &&
                          expectedCommentDesc == actualComment.Description &&
                          expectedCommentPostId == actualComment.PostId);
        }

        [Test]
        public async Task TestVoteWithValidLike()
        {
            await this.AddTwoUsersAsync();
            await this.AddTwoPostsAsync();
            await this.AddTwoCommentsToPostAsync("postId1", "userId1");


            var expectedCommentLikes = 1;
            var expectedCommentDislikes = 0;
            var expectedUserCommentVote = "Like";
            var expectedVoteStatus = "Like";

            var actualVoteStatus = await this.commentsService.CommentVoteAsync("commentId1", "userId1", true);
            var actualCommentLikes = await this.dbContext.Comments.Where(x => x.Id == "commentId1").Select(x => x.Likes).FirstOrDefaultAsync();
            var actualCommentDislikes = await this.dbContext.Comments.Where(x => x.Id == "commentId1").Select(x => x.Dislikes).FirstOrDefaultAsync();
            var actualUserCommentVote = await this.dbContext.UserCommentVotes
                .Where(x => x.ApplicationUserId == "userId1" && x.CommentId == "commentId1").Select(x => x.Value)
                .FirstOrDefaultAsync();

            Assert.IsTrue(expectedCommentDislikes == actualCommentDislikes &&
                          expectedCommentLikes == actualCommentLikes &&
                          expectedUserCommentVote == actualUserCommentVote &&
                          expectedVoteStatus == actualVoteStatus);
        }

        [Test]
        public async Task TestVoteWithValidDislike()
        {
            await this.AddTwoUsersAsync();
            await this.AddTwoPostsAsync();
            await this.AddTwoCommentsToPostAsync("postId1", "userId1");


            var expectedCommentLikes = 0;
            var expectedCommentDislikes = 1;
            var expectedUserCommentVote = "Dislike";
            var expectedVoteStatus = "Dislike";

            var actualVoteStatus = await this.commentsService.CommentVoteAsync("commentId1", "userId1", false);
            var actualCommentLikes = await this.dbContext.Comments.Where(x => x.Id == "commentId1").Select(x => x.Likes).FirstOrDefaultAsync();
            var actualCommentDislikes = await this.dbContext.Comments.Where(x => x.Id == "commentId1").Select(x => x.Dislikes).FirstOrDefaultAsync();
            var actualUserCommentVote = await this.dbContext.UserCommentVotes
                .Where(x => x.ApplicationUserId == "userId1" && x.CommentId == "commentId1").Select(x => x.Value)
                .FirstOrDefaultAsync();

            Assert.IsTrue(expectedCommentDislikes == actualCommentDislikes &&
                          expectedCommentLikes == actualCommentLikes &&
                          expectedUserCommentVote == actualUserCommentVote &&
                          expectedVoteStatus == actualVoteStatus);
        }

        [Test]
        public async Task TestVoteWithLikedCommentThatGotDisliked()
        {
            await this.AddTwoUsersAsync();
            await this.AddTwoPostsAsync();
            await this.AddTwoCommentsToPostAsync("postId1", "userId1");

            await this.commentsService.CommentVoteAsync("commentId1", "userId1", true);

            var expectedCommentLikes = 0;
            var expectedCommentDislikes = 1;
            var expectedUserCommentVote = "Dislike";
            var expectedVoteStatus = "LikeToDislike";

            var actualVoteStatus = await this.commentsService.CommentVoteAsync("commentId1", "userId1", false);
            var actualCommentLikes = await this.dbContext.Comments.Where(x => x.Id == "commentId1").Select(x => x.Likes).FirstOrDefaultAsync();
            var actualCommentDislikes = await this.dbContext.Comments.Where(x => x.Id == "commentId1").Select(x => x.Dislikes).FirstOrDefaultAsync();
            var actualUserCommentVote = await this.dbContext.UserCommentVotes
                .Where(x => x.ApplicationUserId == "userId1" && x.CommentId == "commentId1").Select(x => x.Value)
                .FirstOrDefaultAsync();

            Assert.IsTrue(expectedCommentDislikes == actualCommentDislikes &&
                          expectedCommentLikes == actualCommentLikes &&
                          expectedUserCommentVote == actualUserCommentVote &&
                          expectedVoteStatus == actualVoteStatus);
        }

        [Test]
        public async Task TestVoteWithDislikedCommentThatGotLiked()
        {
            await this.AddTwoUsersAsync();
            await this.AddTwoPostsAsync();
            await this.AddTwoCommentsToPostAsync("postId1", "userId1");

            await this.commentsService.CommentVoteAsync("commentId1", "userId1", false);

            var expectedCommentLikes = 1;
            var expectedCommentDislikes = 0;
            var expectedUserCommentVote = "Like";
            var expectedVoteStatus = "DislikeToLike";

            var actualVoteStatus = await this.commentsService.CommentVoteAsync("commentId1", "userId1", true);
            var actualCommentLikes = await this.dbContext.Comments.Where(x => x.Id == "commentId1").Select(x => x.Likes).FirstOrDefaultAsync();
            var actualCommentDislikes = await this.dbContext.Comments.Where(x => x.Id == "commentId1").Select(x => x.Dislikes).FirstOrDefaultAsync();
            var actualUserCommentVote = await this.dbContext.UserCommentVotes
                .Where(x => x.ApplicationUserId == "userId1" && x.CommentId == "commentId1").Select(x => x.Value)
                .FirstOrDefaultAsync();

            Assert.IsTrue(expectedCommentDislikes == actualCommentDislikes &&
                          expectedCommentLikes == actualCommentLikes &&
                          expectedUserCommentVote == actualUserCommentVote &&
                          expectedVoteStatus == actualVoteStatus);
        }

        [Test]
        public async Task TestVoteWithLikedCommentThatGotLikedAgain()
        {
            await this.AddTwoUsersAsync();
            await this.AddTwoPostsAsync();
            await this.AddTwoCommentsToPostAsync("postId1", "userId1");

            await this.commentsService.CommentVoteAsync("commentId1", "userId1", true);

            var expectedCommentLikes = 0;
            var expectedCommentDislikes = 0;
            string expectedUserCommentVote = null;
            var expectedVoteStatus = "UnLike";

            var actualVoteStatus = await this.commentsService.CommentVoteAsync("commentId1", "userId1", true);
            var actualCommentLikes = await this.dbContext.Comments.Where(x => x.Id == "commentId1").Select(x => x.Likes).FirstOrDefaultAsync();
            var actualCommentDislikes = await this.dbContext.Comments.Where(x => x.Id == "commentId1").Select(x => x.Dislikes).FirstOrDefaultAsync();
            var actualUserCommentVote = await this.dbContext.UserCommentVotes
                .Where(x => x.ApplicationUserId == "userId1" && x.CommentId == "commentId1").Select(x => x.Value)
                .FirstOrDefaultAsync();

            Assert.IsTrue(expectedCommentDislikes == actualCommentDislikes &&
                          expectedCommentLikes == actualCommentLikes &&
                          expectedUserCommentVote == actualUserCommentVote &&
                          expectedVoteStatus == actualVoteStatus);
        }

        [Test]
        public async Task TestVoteWithDislikedCommentThatGotDislikedAgain()
        {
            await this.AddTwoUsersAsync();
            await this.AddTwoPostsAsync();
            await this.AddTwoCommentsToPostAsync("postId1", "userId1");

            await this.commentsService.CommentVoteAsync("commentId1", "userId1", false);

            var expectedCommentLikes = 0;
            var expectedCommentDislikes = 0;
            string expectedUserCommentVote = null;
            var expectedVoteStatus = "UnDislike";

            var actualVoteStatus = await this.commentsService.CommentVoteAsync("commentId1", "userId1", false);
            var actualCommentLikes = await this.dbContext.Comments.Where(x => x.Id == "commentId1").Select(x => x.Likes).FirstOrDefaultAsync();
            var actualCommentDislikes = await this.dbContext.Comments.Where(x => x.Id == "commentId1").Select(x => x.Dislikes).FirstOrDefaultAsync();
            var actualUserCommentVote = await this.dbContext.UserCommentVotes
                .Where(x => x.ApplicationUserId == "userId1" && x.CommentId == "commentId1").Select(x => x.Value)
                .FirstOrDefaultAsync();

            Assert.IsTrue(expectedCommentDislikes == actualCommentDislikes &&
                          expectedCommentLikes == actualCommentLikes &&
                          expectedUserCommentVote == actualUserCommentVote &&
                          expectedVoteStatus == actualVoteStatus);
        }

        [Test]
        public async Task TestDoesCommentExistWithExistingComment()
        {
            await this.AddTwoUsersAsync();
            await this.AddTwoPostsAsync();
            await this.AddTwoCommentsToPostAsync("postId1", "userId1");

            var actual = await this.commentsService.DoesCommentExistAsync("commentId1");
            var expected = true;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task TestDoesCommentExistWithNoneExistingComment()
        {
            var actual = await this.commentsService.DoesCommentExistAsync("commentId1");
            var expected = false;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task TestTakeCommentsAuthorIdWithExistingComment()
        {
            await this.AddTwoUsersAsync();
            await this.AddTwoPostsAsync();
            await this.AddTwoCommentsToPostAsync("postId1", "userId1");

            var actual = await this.commentsService.TakeAuthorIdAsync("commentId1");
            var expected = "userId1";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task TestTakeCommentsAuthorIdWithNoneExistingComment()
        {
            var actual = await this.commentsService.TakeAuthorIdAsync("commentId1");
            string expected = null;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task TestDeleteCommentWithExistingCommentToDelete()
        {
            await this.AddTwoUsersAsync();
            await this.AddTwoPostsAsync();
            await this.AddTwoCommentsToPostAsync("postId1", "userId1");

            var commentIdToDelete = "commentId1";

            var actual = await this.commentsService.DeleteCommentAsync(commentIdToDelete);
            var isCommentDeleted = this.dbContext.Comments.FirstOrDefault(x => x.Id == commentIdToDelete);

            var expected = commentIdToDelete;

            Assert.IsTrue(expected == actual && isCommentDeleted == null);
        }

        [Test]
        public async Task TestDeleteCommentWithNoneExistingCommentToDelete()
        {
            var commentIdToDelete = "commentId1";

            var actual = await this.commentsService.DeleteCommentAsync(commentIdToDelete);

            string expected = null;

            Assert.IsTrue(expected == actual);
        }

        [Test]
        public async Task TestGetAllComentsWithExistingComments()
        {
            await this.AddTwoUsersAsync();
            await this.AddTwoPostsAsync();
            await this.AddTwoCommentsToPostAsync("postId1", "userId1");

            var actual = await this.commentsService.GetCommentsAsync<CommentsViewModel>(0, 5, "postId1");

            var expected = 2;

            Assert.AreEqual(expected, actual.Count());
        }

        [Test]
        public async Task TestGetAllComentsWithNoneExistingComments()
        {
            var actual = await this.commentsService.GetCommentsAsync<CommentsViewModel>(0, 5, "postId1");

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
    }
}

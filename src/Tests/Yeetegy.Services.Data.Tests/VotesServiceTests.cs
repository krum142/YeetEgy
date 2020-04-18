using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Yeetegy.Data;
using Yeetegy.Data.Models;
using Yeetegy.Data.Repositories;
using Yeetegy.Services.Data.Interfaces;

namespace Yeetegy.Services.Data.Tests
{
    public class VotesServiceTests : BaseServiceTests
    {
        private IVotesService votesService;
        private ApplicationDbContext dbContext;

        [SetUp]
        public async Task SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "votesTestDb").Options;
            this.dbContext = new ApplicationDbContext(options);
            await this.dbContext.Database.EnsureDeletedAsync();

            var postVoteRepository = new EfDeletableEntityRepository<UserPostVote>(this.dbContext);
            var postRepository = new EfDeletableEntityRepository<Post>(this.dbContext);
            this.votesService = new VotesService(postVoteRepository, postRepository);
        }

        [Test]
        public async Task TestVoteWithValidLike()
        {
            await this.AddTwoUsersAsync();
            await this.AddTwoPostsAsync();

            var expectedPostLikes = 1;
            var expectedPostDislikes = 0;
            var expectedUserPostVote = "Like";
            var expectedVoteStatus = "Like";

            var actualVoteStatus = await this.votesService.PostVoteAsync("postId1", "userId1", true);
            var actualPostLikes = await this.dbContext.Posts.Where(x => x.Id == "postId1").Select(x => x.Likes).FirstOrDefaultAsync();
            var actualPostDislikes = await this.dbContext.Posts.Where(x => x.Id == "postId1").Select(x => x.Dislikes).FirstOrDefaultAsync();
            var actualUserPostVote = await this.dbContext.UserPostVotes
                .Where(x => x.ApplicationUserId == "userId1" && x.PostId == "postId1").Select(x => x.Value)
                .FirstOrDefaultAsync();

            Assert.IsTrue(expectedPostDislikes == actualPostDislikes &&
                          expectedPostLikes == actualPostLikes &&
                          expectedUserPostVote == actualUserPostVote &&
                          expectedVoteStatus == actualVoteStatus);
        }

        [Test]
        public async Task TestVoteWithValidDislike()
        {
            await this.AddTwoUsersAsync();
            await this.AddTwoPostsAsync();


            var expectedPostLikes = 0;
            var expectedPostDislikes = 1;
            var expectedUserPostVote = "Dislike";
            var expectedVoteStatus = "Dislike";

            var actualVoteStatus = await this.votesService.PostVoteAsync("postId1", "userId1", false);
            var actualPostLikes = await this.dbContext.Posts.Where(x => x.Id == "postId1").Select(x => x.Likes).FirstOrDefaultAsync();
            var actualPostDislikes = await this.dbContext.Posts.Where(x => x.Id == "postId1").Select(x => x.Dislikes).FirstOrDefaultAsync();
            var actualUserPostVote = await this.dbContext.UserPostVotes
                .Where(x => x.ApplicationUserId == "userId1" && x.PostId == "postId1").Select(x => x.Value)
                .FirstOrDefaultAsync();

            Assert.IsTrue(expectedPostDislikes == actualPostDislikes &&
                          expectedPostLikes == actualPostLikes &&
                          expectedUserPostVote == actualUserPostVote &&
                          expectedVoteStatus == actualVoteStatus);
        }

        [Test]
        public async Task TestVoteWithLikedPostThatGotDisliked()
        {
            await this.AddTwoUsersAsync();
            await this.AddTwoPostsAsync();

            await this.votesService.PostVoteAsync("postId1", "userId1", true);

            var expectedPostLikes = 0;
            var expectedPostDislikes = 1;
            var expectedUserPostVote = "Dislike";
            var expectedVoteStatus = "LikeToDislike";

            var actualVoteStatus = await this.votesService.PostVoteAsync("postId1", "userId1", false);
            var actualPostLikes = await this.dbContext.Posts.Where(x => x.Id == "postId1").Select(x => x.Likes).FirstOrDefaultAsync();
            var actualPostDislikes = await this.dbContext.Posts.Where(x => x.Id == "postId1").Select(x => x.Dislikes).FirstOrDefaultAsync();
            var actualUserPostVote = await this.dbContext.UserPostVotes
                .Where(x => x.ApplicationUserId == "userId1" && x.PostId == "postId1").Select(x => x.Value)
                .FirstOrDefaultAsync();

            Assert.IsTrue(expectedPostDislikes == actualPostDislikes &&
                          expectedPostLikes == actualPostLikes &&
                          expectedUserPostVote == actualUserPostVote &&
                          expectedVoteStatus == actualVoteStatus);
        }

        [Test]
        public async Task TestVoteWithDislikedPostThatGotLiked()
        {
            await this.AddTwoUsersAsync();
            await this.AddTwoPostsAsync();

            await this.votesService.PostVoteAsync("postId1", "userId1", false);

            var expectedPostLikes = 1;
            var expectedPostDislikes = 0;
            var expectedUserPostVote = "Like";
            var expectedVoteStatus = "DislikeToLike";

            var actualVoteStatus = await this.votesService.PostVoteAsync("postId1", "userId1", true);
            var actualPostLikes = await this.dbContext.Posts.Where(x => x.Id == "postId1").Select(x => x.Likes).FirstOrDefaultAsync();
            var actualPostDislikes = await this.dbContext.Posts.Where(x => x.Id == "postId1").Select(x => x.Dislikes).FirstOrDefaultAsync();
            var actualUserPostVote = await this.dbContext.UserPostVotes
                .Where(x => x.ApplicationUserId == "userId1" && x.PostId == "postId1").Select(x => x.Value)
                .FirstOrDefaultAsync();

            Assert.IsTrue(expectedPostDislikes == actualPostDislikes &&
                          expectedPostLikes == actualPostLikes &&
                          expectedUserPostVote == actualUserPostVote &&
                          expectedVoteStatus == actualVoteStatus);
        }

        [Test]
        public async Task TestVoteWithLikedPostThatGotLikedAgain()
        {
            await this.AddTwoUsersAsync();
            await this.AddTwoPostsAsync();

            await this.votesService.PostVoteAsync("postId1", "userId1", true);

            var expectedPostLikes = 0;
            var expectedPostDislikes = 0;
            string expectedUserPostVote = null;
            var expectedVoteStatus = "UnLike";

            var actualVoteStatus = await this.votesService.PostVoteAsync("postId1", "userId1", true);
            var actualPostLikes = await this.dbContext.Posts.Where(x => x.Id == "postId1").Select(x => x.Likes).FirstOrDefaultAsync();
            var actualPostDislikes = await this.dbContext.Posts.Where(x => x.Id == "postId1").Select(x => x.Dislikes).FirstOrDefaultAsync();
            var actualUserPostVote = await this.dbContext.UserPostVotes
                .Where(x => x.ApplicationUserId == "userId1" && x.PostId == "postId1").Select(x => x.Value)
                .FirstOrDefaultAsync();

            Assert.IsTrue(expectedPostDislikes == actualPostDislikes &&
                          expectedPostLikes == actualPostLikes &&
                          expectedUserPostVote == actualUserPostVote &&
                          expectedVoteStatus == actualVoteStatus);
        }

        [Test]
        public async Task TestVoteWithDislikedPostThatGotDislikedAgain()
        {
            await this.AddTwoUsersAsync();
            await this.AddTwoPostsAsync();

            await this.votesService.PostVoteAsync("postId1", "userId1", false);

            var expectedPostLikes = 0;
            var expectedPostDislikes = 0;
            string expectedUserPostVote = null;
            var expectedVoteStatus = "UnDislike";

            var actualVoteStatus = await this.votesService.PostVoteAsync("postId1", "userId1", false);
            var actualPostLikes = await this.dbContext.Posts.Where(x => x.Id == "postId1").Select(x => x.Likes).FirstOrDefaultAsync();
            var actualPostDislikes = await this.dbContext.Posts.Where(x => x.Id == "postId1").Select(x => x.Dislikes).FirstOrDefaultAsync();
            var actualUserPostVote = await this.dbContext.UserPostVotes
                .Where(x => x.ApplicationUserId == "userId1" && x.PostId == "postId1").Select(x => x.Value)
                .FirstOrDefaultAsync();

            Assert.IsTrue(expectedPostDislikes == actualPostDislikes &&
                          expectedPostLikes == actualPostLikes &&
                          expectedUserPostVote == actualUserPostVote &&
                          expectedVoteStatus == actualVoteStatus);
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
    }
}

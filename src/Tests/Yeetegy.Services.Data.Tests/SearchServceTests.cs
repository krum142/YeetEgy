using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Yeetegy.Data;
using Yeetegy.Data.Models;
using Yeetegy.Data.Repositories;
using Yeetegy.Services.Data.Interfaces;
using Yeetegy.Web.ViewModels;

namespace Yeetegy.Services.Data.Tests
{
    public class SearchServceTests : BaseServiceTests
    {
        private ISearchService searchService;
        private ApplicationDbContext dbContext;

        [SetUp]
        public async Task SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "commentsTestDb").Options;
            this.dbContext = new ApplicationDbContext(options);
            await this.dbContext.Database.EnsureCreatedAsync();

            var tagRepository = new EfDeletableEntityRepository<Tag>(this.dbContext);
            this.searchService = new SearchService(tagRepository);
        }

        [Test]
        public async Task TestGetSerchResultsWithValidPostsAndTags()
        {
            await this.AddTwoUsersAsync();
            await this.AddTwoPostsAsync();
            await this.AddTwoTagsToPostAsync("postId1");

            var actual = await this.searchService.GetSearchResultsAsync<SearchResponseModel>("tagValue1");

            var expectedCount = 1;
            var expectedValue = "tagValue1";

            Assert.IsTrue(expectedCount == actual.Count() && expectedValue == actual.FirstOrDefault().Value);
        }

        [Test]
        public async Task TestGetSerchResultsWithWrongSearchQuery()
        {
            var actual = await this.searchService.GetSearchResultsAsync<SearchResponseModel>("WrongSearch");

            var expectedCount = 0;

            Assert.IsTrue(expectedCount == actual.Count());
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

        private async Task AddTwoTagsToPostAsync(string postId)
        {
            var tags = new List<Tag>()
            {
                new Tag()
                {
                    Id = "tagId1",
                    Value = "tagValue1",
                },
                new Tag()
                {
                    Id = "tagId2",
                    Value = "tagValue2",
                },
            };

            var postTags = new List<PostTag>()
            {
                new PostTag()
                {
                    PostId = postId,
                    TagId = "tagId1",
                },
                new PostTag()
                {
                    PostId = postId,
                    TagId = "tagId2",
                },
            };

            await this.dbContext.Tags.AddRangeAsync(tags);
            await this.dbContext.PostTags.AddRangeAsync(postTags);
            await this.dbContext.SaveChangesAsync();
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Yeetegy.Data;
using Yeetegy.Data.Models;
using Yeetegy.Data.Repositories;
using Yeetegy.Services.Data.Interfaces;

namespace Yeetegy.Services.Data.Tests
{
    public class TagServiceTests
    {
        private ApplicationDbContext dbContext;
        private ITagService tagService;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TagTestDb").Options;

            this.dbContext = new ApplicationDbContext(options);
            this.dbContext.Database.EnsureCreated();

            var tagRepository = new EfDeletableEntityRepository<Tag>(this.dbContext);

            this.tagService = new TagService(tagRepository);
        }

        [Test]
        public async Task TestCreateTagWithValidValue()
        {
            await this.tagService.CreateAsync("Funny");

            var actual = await this.dbContext.Tags.FirstOrDefaultAsync(x => x.Value == "Funny");

            var expected = new Tag()
            {
                Value = "Funny",
            };

            Assert.AreEqual(expected.Value, actual.Value);
        }

        [Test]
        public async Task TestCreateTagWithNullInput()
        {
            await this.tagService.CreateAsync(null);

            var actual = await this.dbContext.Tags.FirstOrDefaultAsync(x => x.Value == null);

            Assert.IsTrue(actual.Id != null);
        }

        [Test]
        public async Task TestIfTagExistsWithExistingTag()
        {
            await this.AddTwoTags();

            var actual = await this.tagService.ExistsAsync("Funny");

            Assert.IsTrue(actual);
        }

        [Test]
        public async Task TestIfTagExistsWithNoneExistingTag()
        {
            var actual = await this.tagService.ExistsAsync("Funny");

            Assert.IsFalse(actual);
        }

        [Test]
        public async Task TestGetIdWithExistingTag()
        {
            await this.AddTwoTags();

            var actual = await this.tagService.GetIdAsync("Funny");
            var expected = "1";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task TestGetIdWithNonExistingTag()
        {
            var actual = await this.tagService.GetIdAsync("NotHere");

            Assert.IsNull(actual);
        }

        [TearDown]
        public void TearDown()
        {
            this.dbContext.Database.EnsureDeleted();
        }

        private async Task AddTwoTags()
        {
            var tags = new List<Tag>()
            {
                new Tag()
                {
                    Id = "1",
                    Value = "Funny",
                },
                new Tag()
                {
                    Id = "2",
                    Value = "Cool",
                },
            };

            await this.dbContext.Tags.AddRangeAsync(tags);
            await this.dbContext.SaveChangesAsync();
        }
    }
}

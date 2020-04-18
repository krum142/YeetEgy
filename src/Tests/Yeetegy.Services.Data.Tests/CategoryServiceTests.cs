using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Yeetegy.Data;
using Yeetegy.Data.Models;
using Yeetegy.Data.Repositories;
using Yeetegy.Services.Data.Interfaces;
using Yeetegy.Services.Data.Tests.TestModels;
using Yeetegy.Services.Mapping;
using Yeetegy.Web.ViewModels;

namespace Yeetegy.Services.Data.Tests
{
    public class CategoryServiceTests : BaseServiceTests
    {
        private ICategoryService categoryService;
        private ApplicationDbContext dbContext;

        [SetUp]
        public async Task SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "CategoryTestDb").Options;

            this.dbContext = new ApplicationDbContext(options);
            await this.dbContext.Database.EnsureDeletedAsync();

            var categoryRepo = new EfDeletableEntityRepository<Category>(this.dbContext);
            var fakeCloudinary = new FakeCloudinary();
            this.categoryService = new CategoryService(categoryRepo, fakeCloudinary);

           
        }

        private async Task AddTwoCategorysAsync()
        {
            var categories = new List<Category>()
            {
                new Category()
                {
                    Id = "1",
                    Name = "Funny",
                    ImageUrl = "Funny.Url",
                },
                new Category()
                {
                    Id = "2",
                    Name = "Cool",
                    ImageUrl = "Cool.Url",
                },
            };

            await this.dbContext.Categories.AddRangeAsync(categories);
            await this.dbContext.SaveChangesAsync();
        }

        [Test]
        public async Task TestCreateCategoryWithValidInput()
        {
            var newCategoryName = "Funny";
            IFormFile file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", "dummy.png");
            var expected = new Category()
            {
                Name = "Funny",
                ImageUrl = "FakeCloudinaryUrl",
            };

            await this.categoryService.CreateAsync(newCategoryName, file);

            var actual = await this.dbContext.Categories.FirstOrDefaultAsync(x => x.Name == newCategoryName);

            Assert.True(actual.Name == expected.Name && actual.ImageUrl == expected.ImageUrl); ;
        }

        [Test]
        public async Task TestCreateCategoryWithNullFile()
        {
            var newCategoryName = "Funny";
            var expected = new Category()
            {
                Name = "Funny",
                ImageUrl = null,
            };

            await this.categoryService.CreateAsync(newCategoryName, null);
            var actual = await this.dbContext.Categories.FirstOrDefaultAsync(x => x.Name == newCategoryName);

            Assert.True(actual.Name == expected.Name && actual.ImageUrl == expected.ImageUrl);
        }

        [Test]
        public async Task TestOfGettingAllCategorys()
        {
            await this.AddTwoCategorysAsync();

            var expected = new List<CategoryViewModel>()
            {
                new CategoryViewModel()
                {
                    Name = "Funny",
                },
                new CategoryViewModel()
                {
                    Name = "Cool",
                },
            };

            var actual = await this.categoryService.GetAllAsync<CategoryViewModel>();

            var actualActual = actual.ToList();

            var AreSame = true;

            for (int i = 0; i < expected.Count(); i++)
            {
                if (actualActual[i].Name != expected[i].Name &&
                    actualActual[i].ImageUrl == expected[i].ImageUrl)
                {
                    AreSame = false;
                    break;
                }
            }

            var x = AreSame;

            Assert.IsTrue(AreSame);
        }

        [Test]
        public async Task TestGetAllCategorysWithEmptyDb()
        {
            var x = await this.categoryService.GetAllAsync<CategoryViewModel>();

            Assert.IsEmpty(x);
        }

        [Test]
        public async Task TestOfGettingAllListItemsCategorys()
        {
            await this.AddTwoCategorysAsync();

            var expected = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text = "Funny",
                },
                new SelectListItem()
                {
                    Text = "Cool",
                },
            };

            var actual = await this.categoryService.GetAllListItemsAsync();

            var actualActual = actual.ToList();

            var AreSame = true;

            for (int i = 0; i < expected.Count(); i++)
            {
                if (actualActual[i].Value != expected[i].Value)
                {
                    AreSame = false;
                    break;
                }
            }

            var x = AreSame;

            Assert.IsTrue(AreSame);
        }

        [Test]
        public async Task TestGetAllListItemsCategorysWithEmptyDb()
        {
            var x = await this.categoryService.GetAllListItemsAsync();

            Assert.IsEmpty(x);
        }

        [Test]
        public async Task TestDeletingOfCategory()
        {
            await this.AddTwoCategorysAsync();

            await this.categoryService.DeleteAsync("Funny");

            var expected = 1;
            var actual = this.dbContext.Categories.Count();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task TestIsThereAnyWithMatchingValue()
        {
            await this.AddTwoCategorysAsync();

            Assert.IsTrue(await this.categoryService.IsThereAnyAsync("Funny"));
        }

        [Test]
        public async Task TestIsThereAnyWithNonMatchingValue()
        {
            await this.AddTwoCategorysAsync();

            Assert.IsFalse(await this.categoryService.IsThereAnyAsync("Pesho"));
        }

        [Test]
        public async Task TestGetIdWithValidInput()
        {
            await this.AddTwoCategorysAsync();

            var actualId = await this.categoryService.GetIdAsync("Funny");
            var expectedId = "1";

            Assert.AreEqual(expectedId, actualId);
        }

        [Test]
        public async Task TestGetIdWithEmptyDb()
        {
            var actualId = await this.categoryService.GetIdAsync("Funny");

            string expectedId = null;

            Assert.AreEqual(expectedId, actualId);
        }

        [Test]
        public async Task TestGetIdWithNoneExistingElement()
        {
            await this.AddTwoCategorysAsync();

            var actualId = await this.categoryService.GetIdAsync("Banana");

            string expectedId = null;

            Assert.AreEqual(expectedId, actualId);
        }

        [Test]
        public async Task TestGetImgWithExistingImage()
        {
            await this.AddTwoCategorysAsync();

            var actualUrl = await this.categoryService.GetImgAsync("Funny");
            var expectedUrl = "Funny.Url";

            Assert.AreEqual(expectedUrl, actualUrl);
        }

        [Test]
        public async Task TestGetImgWithEmptyDb()
        {

            var actualUrl = await this.categoryService.GetImgAsync("Funny");
            string expectedUrl = null;

            Assert.AreEqual(expectedUrl, actualUrl);
        }

        [Test]
        public async Task TestGetImgWithNoneExistantCategory()
        {
            await this.AddTwoCategorysAsync();

            var actualUrl = await this.categoryService.GetImgAsync("Banana");
            string expectedUrl = null;

            Assert.AreEqual(expectedUrl, actualUrl);
        }

        [TearDown]
        public void TearDown()
        {
            this.dbContext.Database.EnsureDeleted();
        }
    }
}

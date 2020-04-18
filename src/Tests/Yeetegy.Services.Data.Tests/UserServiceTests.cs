using System.Collections.Generic;
using System.IO;
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
using Yeetegy.Web.ViewModels.PostViewModels;

namespace Yeetegy.Services.Data.Tests
{
    public class UserServiceTests : BaseServiceTests
    {
        private IUserService userService;
        private ApplicationDbContext dbContext;
        private FakeCloudinary cloudinaryService;

        [SetUp]
        public async Task SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "userServiceTestDb").Options;
            this.dbContext = new ApplicationDbContext(options);
            await this.dbContext.Database.EnsureDeletedAsync();

            var userRepository = new EfDeletableEntityRepository<ApplicationUser>(this.dbContext);
            this.cloudinaryService = new FakeCloudinary();
            this.userService = new UserService(userRepository, cloudinaryService);
        }

        [Test]
        public async Task TestDoesUserExistWithExistingUser()
        {
            await this.AddTwoUsersAsync();

            var actual = await this.userService.ExistsAsync("Pesho");

            Assert.IsTrue(actual);
        }

        [Test]
        public async Task TestDoesUserExistWithNoneExistingUser()
        {
            var actual = await this.userService.ExistsAsync("Pesho");
            Assert.IsFalse(actual);
        }

        [Test]
        public async Task TestDoesUserExistWithNullUsername()
        {
            var actual = await this.userService.ExistsAsync(null);
            Assert.IsFalse(actual);
        }

        [Test]
        public async Task TestChangeUserAvatarPictureWithExistingUserAndValidInput()
        {
            await this.AddTwoUsersAsync();

            var fileName = "Img";
            IFormFile file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, fileName, "dummy.png");

            var actual = await this.userService.ChangeAvatarPictureAsync("Pesho", file, "DefaultAvatarUrl1");
            var expected = fileName;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task TestChangeUserAvatarPictureWithNoneExistingUserAndValidInput()
        {
            var fileName = "Img";
            IFormFile file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, fileName, "dummy.png");

            var actual = await this.userService.ChangeAvatarPictureAsync("Pesho", file, "DefaultAvatarUrl1");

            Assert.IsNull(actual);
        }

        [Test]
        public async Task TestChangeUserAvatarPictureWithExistingUserAndCloudinaryReturnNull()
        {
            await this.AddTwoUsersAsync();

            this.cloudinaryService.ReturnNullOnCreate = true;

            var fileName = "Img";
            IFormFile file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, fileName, "dummy.png");

            var actual = await this.userService.ChangeAvatarPictureAsync("Pesho", file, "DefaultAvatarUrl1");

            Assert.IsNull(actual);
        }

        [Test]
        public async Task TestChangeUserAvatarPictureWithWrongUsernameAndValidInput()
        {
            await this.AddTwoUsersAsync();

            var fileName = "Img";
            IFormFile file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, fileName, "dummy.png");

            var actual = await this.userService.ChangeAvatarPictureAsync("WrongUsernName", file, "DefaultAvatarUrl1");

            Assert.IsNull(actual);
        }

        [Test]
        public async Task TestMarkUserAsDeletedWithExistingUser()
        {
            await this.AddTwoUsersAsync();
            var user = await this.dbContext.Users.FirstOrDefaultAsync(x => x.Id == "userId1");

            var isUserMarkedAsDeleted = await this.userService.MarkAsDeletedAsync(user);

            var actual = user.IsDeleted;

            Assert.IsTrue(actual && isUserMarkedAsDeleted);
        }

        [Test]
        public async Task TestMarkUserAsDeletedWithNoneExistingUser()
        {
            var isUserMarkedAsDeleted = await this.userService.MarkAsDeletedAsync(null);

            Assert.IsFalse(isUserMarkedAsDeleted);
        }

        [Test]
        public async Task TestGetUserIdWithValidUser()
        {
            await this.AddTwoUsersAsync();

            var actual = await this.userService.GetIdAsync("Pesho");

            var expected = "userId1";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task TestGetUserIdWithNoneValidUser()
        {
            var actual = await this.userService.GetIdAsync("WrongUser");

            Assert.IsNull(actual);
        }

        [Test]
        public async Task TestGetUserByNameWithValidUser()
        {
            await this.AddTwoUsersAsync();

            var actual = await this.userService.GetUserByNameAsync<UserPageContentsModel>("Pesho");

            var expectedAvatarUrl = "DefaultAvatarUrl1";
            var expectedUsername = "Pesho";

            Assert.IsTrue(expectedUsername == actual.Username && expectedAvatarUrl == actual.AvatarUrl);
        }

        [Test]
        public async Task TestGetUserByNameWithNoneValidUser()
        {
            Assert.IsNull(await this.userService.GetUserByNameAsync<UserPageContentsModel>("Pesho"));
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
                    NormalizedUserName = "PESHO",
                    AvatarUrl = "DefaultAvatarUrl1",
                },
                new ApplicationUser()
                {
                    Id = "userId2",
                    UserName = "Ivan",
                    NormalizedUserName = "IVAN",
                    AvatarUrl = "DefaultAvatarUrl2",
                },
            };

            await this.dbContext.Users.AddRangeAsync(users);
            await this.dbContext.SaveChangesAsync();
        }
    }
}

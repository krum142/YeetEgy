using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Yeetegy.Common;
using Yeetegy.Data.Common.Repositories;
using Yeetegy.Data.Models;
using Yeetegy.Services.Data.Interfaces;
using Yeetegy.Services.Mapping;

namespace Yeetegy.Services.Data
{
    public class UserService : IUserService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;
        private readonly ICloudinaryService cloudinaryService;

        public UserService(IDeletableEntityRepository<ApplicationUser> userRepository, ICloudinaryService cloudinaryService)
        {
            this.userRepository = userRepository;
            this.cloudinaryService = cloudinaryService;
        }

        public async Task<bool> ExistsAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return false;
            }

            return await this.userRepository
                .AllAsNoTracking()
                .AnyAsync(x => x.NormalizedUserName == username.ToUpper());
        }

        public async Task<string> ChangeAvatarPictureAsync(string username, IFormFile newPicture, string oldPictureLink)
        {
            if (oldPictureLink == null || !await this.ExistsAsync(username))
            {
                return null;
            }

            if (oldPictureLink != GlobalConstants.DefaultUserImg)
            {
                var oldPictureId = oldPictureLink.Split("/").LastOrDefault()?.Split('.').FirstOrDefault();
                this.cloudinaryService.DeleteCloudinary(oldPictureId);
            }

            var newImageLink = await this.cloudinaryService.SaveCloudinaryAsync(newPicture);
            var user = await this.userRepository.All().FirstOrDefaultAsync(x => x.UserName == username);
            if (newImageLink == null || user == null)
            {
                return null;
            }

            user.AvatarUrl = newImageLink;
            await this.userRepository.SaveChangesAsync();

            return newImageLink;
        }

        public async Task<bool> MarkAsDeletedAsync(ApplicationUser user)
        {
            try
            {
                this.userRepository.Delete(user);
                var x = await this.userRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<string> GetIdAsync(string username)
        {
            return await this.userRepository
                .AllAsNoTracking()
                .Where(x => x.NormalizedUserName == username.ToUpper())
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<T> GetUserByNameAsync<T>(string username)
        {
            return await this.userRepository.AllAsNoTracking().Where(X => X.NormalizedUserName == username.ToUpper())
                .To<T>().FirstOrDefaultAsync();
        }
    }
}

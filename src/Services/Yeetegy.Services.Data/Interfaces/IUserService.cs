using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Yeetegy.Data.Models;

namespace Yeetegy.Services.Data.Interfaces
{
    public interface IUserService
    {
        Task<bool> ExistsAsync(string username);

        Task<string> GetIdAsync(string username);

        Task<T> GetUserByNameAsync<T>(string username);

        Task<string> ChangeAvatarPictureAsync(string username, IFormFile newPicture, string oldPictureLink);

        Task<bool> MarkAsDeletedAsync(ApplicationUser user);
    }
}

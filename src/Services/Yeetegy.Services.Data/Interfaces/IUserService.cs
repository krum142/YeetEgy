using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Yeetegy.Services.Data.Interfaces
{
    public interface IUserService
    {
        Task<bool> ExistsAsync(string username);

        Task<string> GetIdAsync(string username);

        Task<T> GetUserByNameAsync<T>(string username);

        Task<string> ChangeAvatarPicture(string username, IFormFile newPicture, string oldPictureLink);
    }
}

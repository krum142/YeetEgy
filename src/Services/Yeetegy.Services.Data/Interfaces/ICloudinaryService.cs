using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Yeetegy.Services.Data.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> SaveCloudinaryAsync(IFormFile image);
    }
}
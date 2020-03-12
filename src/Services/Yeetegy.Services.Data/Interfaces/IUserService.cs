using System.Threading.Tasks;
using Yeetegy.Data.Models;

namespace Yeetegy.Services.Data.Interfaces
{
    public interface IUserService
    {
        Task AddPostAsync(Post post, string userId);
    }
}
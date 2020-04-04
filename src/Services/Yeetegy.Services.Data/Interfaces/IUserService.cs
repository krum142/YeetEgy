using System.Threading.Tasks;

namespace Yeetegy.Services.Data.Interfaces
{
    public interface IUserService
    {
        Task<bool> ExistsAsync(string username);

        Task<string> GetIdAsync(string username);
    }
}

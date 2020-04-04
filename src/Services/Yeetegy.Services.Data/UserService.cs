using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Yeetegy.Data.Common.Repositories;
using Yeetegy.Data.Models;
using Yeetegy.Services.Data.Interfaces;

namespace Yeetegy.Services.Data
{
    public class UserService : IUserService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;

        public UserService(IDeletableEntityRepository<ApplicationUser> userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<bool> ExistsAsync(string username)
        {
            return await this.userRepository.AllAsNoTracking().AnyAsync(x => x.NormalizedUserName == username.ToUpper());
        }

        public async Task<string> GetIdAsync(string username)
        {
            return await this.userRepository.AllAsNoTracking().Where(x => x.NormalizedUserName == username.ToUpper()).Select(x => x.Id)
                .FirstOrDefaultAsync();
        }

    }
}

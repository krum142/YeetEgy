using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Yeetegy.Data.Common.Repositories;
using Yeetegy.Data.Models;
using Yeetegy.Services.Data.Interfaces;

namespace Yeetegy.Services.Data
{
    public class TagService : ITagService
    {
        private readonly IDeletableEntityRepository<Tag> tagRepository;

        public TagService(IDeletableEntityRepository<Tag> tagRepository)
        {
            this.tagRepository = tagRepository;
        }

        public async Task<string> CreateAsync(string tagName)
        {
            var tag = new Tag()
            {
                Value = tagName,
            };

            await this.tagRepository.AddAsync(tag);
            await this.tagRepository.SaveChangesAsync();

            return tag.Id;
        }

        public async Task<bool> ExistsAsync(string tagValue)
        {
            return await this.tagRepository.AllAsNoTracking().AnyAsync(x => x.Value == tagValue);
        }
    }
}

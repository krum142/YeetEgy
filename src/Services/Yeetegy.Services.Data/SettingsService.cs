using Yeetegy.Services.Data.Interfaces;

namespace Yeetegy.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;

    using Yeetegy.Data.Common.Repositories;
    using Yeetegy.Data.Models;
    using Mapping;

    public class SettingsService : ISettingsService
    {
        private readonly IDeletableEntityRepository<Setting> settingsRepository;

        public SettingsService(IDeletableEntityRepository<Setting> settingsRepository)
        {
            this.settingsRepository = settingsRepository;
        }

        public int GetCount()
        {
            return settingsRepository.All().Count();
        }

        public IEnumerable<T> GetAll<T>()
        {
            return settingsRepository.All().To<T>().ToList();
        }
    }
}

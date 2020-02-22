using System.Collections.Generic;

namespace Yeetegy.Services.Data.Interfaces
{
    public interface ISettingsService
    {
        int GetCount();

        IEnumerable<T> GetAll<T>();
    }
}

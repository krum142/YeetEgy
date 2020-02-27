using System.Collections.Generic;
using System.Threading.Tasks;
using Yeetegy.Data.Models;

namespace Yeetegy.Services.Data.Interfaces
{
    public interface ICategoryService
    {
        Task CreateAsync(string name);

        IEnumerable<string> GetAll();

        string GetId(string name);

        bool IsThereAny(string categoryName);
    }
}
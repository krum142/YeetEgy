using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Yeetegy.Data.Models;
using Yeetegy.Web.ViewModels;

namespace Yeetegy.Services.Data.Interfaces
{
    public interface ICategoryService
    {
        Task CreateAsync(string name, IFormFile image);

        IEnumerable<CategoryViewModel> GetAll();

        string GetId(string name);

        bool IsThereAny(string categoryName);

        Task DeleteAsync(string name);
    }
}
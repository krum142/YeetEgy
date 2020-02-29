using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Yeetegy.Web.ViewModels;

namespace Yeetegy.Services.Data.Interfaces
{
    public interface ICategoryService
    {
        Task CreateAsync(string name, IFormFile image);

        IEnumerable<CategoryViewModel> GetAll();

        IEnumerable<SelectListItem> GetAllListItems();

        string GetId(string name);

        bool IsThereAny(string categoryName);

        Task DeleteAsync(string name);
    }
}
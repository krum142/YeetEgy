using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Yeetegy.Data.Models;
using Yeetegy.Web.ViewModels;

namespace Yeetegy.Services.Data.Interfaces
{
    public interface ICategoryService
    {
        Task CreateAsync(string name, IFormFile image);

        CategorysViewModel GetAll();

        IEnumerable<SelectListItem> GetAllListItems();

        string GetId(string categoryName);

        string GetImg(string name);

        bool IsThereAny(string categoryName);

        Task DeleteAsync(string name);
    }
}
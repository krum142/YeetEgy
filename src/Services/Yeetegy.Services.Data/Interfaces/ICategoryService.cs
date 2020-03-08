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

        IEnumerable<T> GetAll<T>();

        IEnumerable<SelectListItem> GetAllListItems();

        Task<string> GetIdAsync(string categoryName);

        Task<string> GetImgAsync(string categoryName); // delete it if you think its worthless

        Task<bool> IsThereAnyAsync(string categoryName);

        Task DeleteAsync(string name);
    }
}
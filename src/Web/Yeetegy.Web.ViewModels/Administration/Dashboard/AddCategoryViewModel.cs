using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Yeetegy.Web.ViewModels.Administration.Dashboard
{
    public class AddCategoryViewModel
    {
        public IFormFile File { get; set; }

        [MaxLength(30)]
        public string Category { get; set; }
    }
}
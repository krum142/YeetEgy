using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Serialization;
using Yeetegy.Web.Infrastructure.ValidationAtributes;

namespace Yeetegy.Web.ViewModels.Administration.Dashboard
{
    public class AddCategoryViewModel
    {
        [Required(ErrorMessage = "Image Is Required")]
        [FileValidationAttribute]
        public IFormFile File { get; set; }

        [Required(ErrorMessage = "Category Name is required!")]
        [MaxLength(30,ErrorMessage = "Category name cannot be above 30 symbols!")]
        public string Category { get; set; }
    }
}
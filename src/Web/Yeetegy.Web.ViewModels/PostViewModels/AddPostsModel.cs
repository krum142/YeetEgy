using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Yeetegy.Web.Infrastructure.ValidationAtributes;

namespace Yeetegy.Web.ViewModels.PostViewModels
{
    public class AddPostsModel
    {
        [Required(ErrorMessage = "Picture is Required!")]
        [FileValidation]
        public IFormFile File { get; set; }

        [Required(ErrorMessage = "Tittle Is Required!")]
        [MaxLength(200, ErrorMessage = "Tittle cannot be above 200 symbols!")]
        public string Tittle { get; set; }

        //[RegularExpression("#[A-Za-z0-9]{1,30}")]
        [TagsValidation]
        public string Tags { get; set; }

        [Required]
        public string Category { get; set; }

        public IEnumerable<SelectListItem> Categorys { get; set; }
    }
}

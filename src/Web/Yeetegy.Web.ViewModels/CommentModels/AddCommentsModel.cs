using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Yeetegy.Web.Infrastructure.ValidationAtributes;

namespace Yeetegy.Web.ViewModels.CommentModels
{
    public class AddCommentsModel
    {
        [Required(ErrorMessage = "Description is Required!")]
        public string Description { get; set; }

        [Required]
        public string PostId { get; set; }

        [FileValidation]
        public IFormFile File { get; set; }
    }
}

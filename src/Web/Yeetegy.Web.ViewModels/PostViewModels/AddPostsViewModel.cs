using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Yeetegy.Web.ViewModels.PostViewModels
{
    public class AddPostsViewModel
    {
        [Required]
        public IFormFile File { get; set; }

        [Required]
        [MaxLength(200)]
        public string Tittle { get; set; }

        [Required]
        public string Category { get; set; }

        public IEnumerable<SelectListItem> Categorys { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace Yeetegy.Web.ViewModels.PostViewModels
{
    public class DeletePostInputModel
    {
        [Required]
        public string Id { get; set; }
    }
}

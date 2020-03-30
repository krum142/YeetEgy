using System.ComponentModel.DataAnnotations;

namespace Yeetegy.Web.ViewModels.CommentModels
{
    public class DeleteCommentInputModel
    {
        [Required]
        public string Id { get; set; }
    }
}
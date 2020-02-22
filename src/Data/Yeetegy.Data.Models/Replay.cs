using System;
using System.ComponentModel.DataAnnotations;
using Yeetegy.Data.Common.Models;

namespace Yeetegy.Data.Models
{
    public class Replay : BaseDeletableModel<string>
    {
        public Replay()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        [MaxLength(2500)]
        public string Description { get; set; }

        [Required]
        public string CommentId { get; set; }

        public Comment Comment { get; set; }

        [MaxLength(1000)]
        public string ImgUrl { get; set; }

        public int Likes { get; set; }

        public int Dislikes { get; set; }
    }
}
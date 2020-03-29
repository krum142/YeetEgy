using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Yeetegy.Data.Common.Models;

namespace Yeetegy.Data.Models
{
    public class Comment : BaseDeletableModel<string>
    {
        public Comment()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Replays = new HashSet<Comment>();
        }

        [Required]
        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public string ReplayId { get; set; }

        public Comment Replay { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; }

        [Required]
        public string PostId { get; set; }

        public Post Post { get; set; }

        [MaxLength(1000)]
        public string ImgUrl { get; set; }

        public int Likes { get; set; }

        public int Dislikes { get; set; }

        public ICollection<Comment> Replays { get; set; }
    }
}

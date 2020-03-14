using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Yeetegy.Data.Common.Models;

namespace Yeetegy.Data.Models
{
    public class Post : BaseDeletableModel<string>
    {
        public Post()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Comments = new HashSet<Comment>();
            this.UserVotes = new HashSet<UserPostVote>();
        }

        [Required]
        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        [MaxLength(200)]
        public string Tittle { get; set; }

        [Required]
        [MaxLength(1000)]
        public string ImgUrl { get; set; }

        public int Likes { get; set; }

        public int Dislikes { get; set; }

        [Required]
        public string CategoryId { get; set; }

        public Category Category { get; set; }

        public ICollection<UserPostVote> UserVotes { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}

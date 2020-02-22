using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Yeetegy.Data.Common.Models;

namespace Yeetegy.Data.Models
{
    public class Category : BaseDeletableModel<string>
    {
        public Category()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Posts = new HashSet<Post>();
        }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        public byte[] Image { get; set; }

        public ICollection<Post> Posts { get; set; }
    }
}

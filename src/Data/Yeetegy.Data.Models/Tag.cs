using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Yeetegy.Data.Common.Models;

namespace Yeetegy.Data.Models
{
    public class Tag : BaseDeletableModel<string>
    {
        public Tag()
        {
            this.Id = Guid.NewGuid().ToString();
            this.PostTags = new HashSet<PostTag>();
        }

        [MaxLength(15)]
        public string Value { get; set; }

        public ICollection<PostTag> PostTags { get; set; }
    }
}

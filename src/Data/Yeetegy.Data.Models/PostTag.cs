using System;

using Yeetegy.Data.Common.Models;

namespace Yeetegy.Data.Models
{
    public class PostTag : BaseModel<string>
    {
        public PostTag()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string TagId { get; set; }

        public Tag Tag { get; set; }

        public string PostId { get; set; }

        public Post Post { get; set; }
    }
}

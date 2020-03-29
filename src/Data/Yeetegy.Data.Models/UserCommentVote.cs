using System;

using Yeetegy.Data.Common.Models;

namespace Yeetegy.Data.Models
{
    public class UserCommentVote : BaseDeletableModel<string>
    {
        public UserCommentVote()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string CommentId { get; set; }

        public Comment Comment { get; set; }

        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public string Value { get; set; }
    }
}

using Yeetegy.Data.Common.Models;

namespace Yeetegy.Data.Models
{
    public class UserLikePost : BaseDeletableModel<string>
    {
        public string PostId { get; set; }

        public Post Post { get; set; }

        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
    }
}
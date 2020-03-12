namespace Yeetegy.Data.Models
{
    public class UserLikePost
    {
        public string PostId { get; set; }

        public Post Post { get; set; }

        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
    }
}
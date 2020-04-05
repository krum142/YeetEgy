using Yeetegy.Data.Models;
using Yeetegy.Services.Mapping;

namespace Yeetegy.Web.ViewModels.PostViewModels
{
    public class UserPageContentsModel : IMapFrom<ApplicationUser>
    {
        public string Username { get; set; }

        public string AvatarUrl { get; set; }
    }
}

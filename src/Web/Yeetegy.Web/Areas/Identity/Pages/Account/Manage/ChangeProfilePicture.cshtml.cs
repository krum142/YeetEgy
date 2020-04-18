using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Yeetegy.Data.Models;
using Yeetegy.Services.Data.Interfaces;
using Yeetegy.Web.Infrastructure.ValidationAtributes;

namespace Yeetegy.Web.Areas.Identity.Pages.Account.Manage
{
    public class ChangeProfilePicture : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserService userService;

        public ChangeProfilePicture(UserManager<ApplicationUser> userManager, IUserService userService)
        {
            this.userManager = userManager;
            this.userService = userService;
        }

        [Required(ErrorMessage = "Profile Picture is Required")]
        [BindProperty]
        [FileValidation]
        public IFormFile NewProfilePicture { get; set; }

        public string AvatarUrl { get; set; }

        private async Task LoadAsync(ApplicationUser user)
        {
            var avatarUrl = await this.userManager.Users.Where(x => x.UserName == this.User.Identity.Name).Select(x => x.AvatarUrl).FirstOrDefaultAsync();

            this.AvatarUrl = avatarUrl;

        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
            }

            await this.LoadAsync(user);
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
            }

            if (!this.ModelState.IsValid)
            {
                await this.LoadAsync(user);
                return this.Page();
            }

            var userUsername = this.User.Identity.Name;
            var oldPictureUrl = this.userManager.Users.Where(x => x.UserName == userUsername)
                .Select(x => x.AvatarUrl).FirstOrDefault();
            var newAvatarUrl = await this.userService.ChangeAvatarPictureAsync(userUsername, this.NewProfilePicture, oldPictureUrl);

            this.AvatarUrl = newAvatarUrl;

            return this.RedirectToPage();
        }
    }
}

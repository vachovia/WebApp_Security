using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Settings;
using WebApp.ViewModels;

namespace WebApp.Pages.Account
{
    [Authorize(Roles = $"{SD.AdminRole}")]
    public class UserProfileListModel : PageModel
    {
        private readonly UserManager<AppUser> _UserManager;
        private readonly RoleManager<IdentityRole> _RoleManager;

        [BindProperty]
        public List<UserProfileViewModel> UserProfileList { get; set; }

        public UserProfileListModel(UserManager<AppUser> UserManager, RoleManager<IdentityRole> RoleManager)
        {
            _UserManager = UserManager;
            _RoleManager = RoleManager;
            UserProfileList = new List<UserProfileViewModel>();
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var users = await _UserManager.Users.ToListAsync();

            foreach (var user in users)
            {
                var roles = await _UserManager.GetRolesAsync(user);

                var u = new UserProfileViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Department = user.Department,
                    Position = user.Position,
                    Roles = string.Join(", ", roles)
                };

                UserProfileList.Add(u);
            }

            return Page();
        }
    }
}

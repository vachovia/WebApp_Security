using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using WebApp.Data;
using WebApp.Settings;
using WebApp.ViewModels;

namespace WebApp.Pages.Account
{
    [Authorize]
    public class UserProfileModel : PageModel
    {
        private readonly UserManager<AppUser> _UserManager;
        private readonly RoleManager<IdentityRole> _RoleManager;

        [BindProperty]
        public UserProfileViewModel UserProfile { get; set; }

        [BindProperty]
        public List<string?> AppRoles { get; set; } = new();

        [BindProperty]
        public string? SuccessMessage { get; set; } // after .NET 6 if we don't mark with ? it will be considered as required

        public UserProfileModel(UserManager<AppUser> UserManager, RoleManager<IdentityRole> RoleManager)
        {
            _UserManager = UserManager;
            _RoleManager = RoleManager;
            UserProfile = new UserProfileViewModel();
        }

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            SuccessMessage = string.Empty;

            AppRoles = await _RoleManager.Roles.Select(r => r.Name).ToListAsync();

            var (user, departmentClaim, positionClaim, userRoles) = await GetUserInfoAsync(id);

            if (user != null)
            {
                UserProfile.Id = user.Id;
                UserProfile.FirstName = user.FirstName;
                UserProfile.LastName = user.LastName;
                UserProfile.Email = user.Email ?? string.Empty;
                UserProfile.Department = user.Department ?? string.Empty;
                UserProfile.Position =user.Position ?? string.Empty;
                UserProfile.Roles = userRoles != null ? string.Join(",", userRoles) : string.Empty;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            AppRoles = await _RoleManager.Roles.Select(r => r.Name).ToListAsync();

            if (!ModelState.IsValid) return Page();

            try
            {
                string? id = UserProfile.Id;

                var (user, departmentClaim, positionClaim, userRoles) = await GetUserInfoAsync(id);

                if (user != null && userRoles != null)
                {
                    user.FirstName = UserProfile.FirstName;
                    user.LastName = UserProfile.LastName;
                    user.Department = UserProfile.Department;
                    user.Position = UserProfile.Position;

                    await _UserManager.RemoveFromRolesAsync(user, userRoles);

                    foreach (var role in UserProfile.Roles.Split(",").ToArray())
                    {
                        var roleToAdd = await _RoleManager.Roles.FirstOrDefaultAsync(r => r.Name == role);

                        if (roleToAdd != null)
                        {
                            await _UserManager.AddToRoleAsync(user, role);
                        }
                    }

                    if (departmentClaim != null && departmentClaim.Value != UserProfile.Department)
                    {
                        await _UserManager.ReplaceClaimAsync(user, departmentClaim, new Claim(departmentClaim.Type, UserProfile.Department));
                    }

                    if (positionClaim != null && positionClaim.Value != UserProfile.Position)
                    {
                        await _UserManager.ReplaceClaimAsync(user, positionClaim, new Claim(positionClaim.Type, UserProfile.Position));
                    }
                }               
            }
            catch
            {
                ModelState.AddModelError("UserProfile", "Error occured during updating user profile.");

                return Page();
            }

            SuccessMessage = "The user profile is updated successfully.";

            return Page();
        }

        private async Task<(AppUser? user, Claim? departmentClaim, Claim? positionClaim, IList<string>? roles)> GetUserInfoAsync(string? id)
        {
            AppUser? user;

            if (string.IsNullOrEmpty(id))
            {
                user = await _UserManager.FindByNameAsync(User.Identity?.Name ?? string.Empty);
            }
            else
            {
                user = await _UserManager.FindByIdAsync(id);
            }

            if (user != null)
            {
                var claims = await _UserManager.GetClaimsAsync(user);
                var departmentClaim = claims.FirstOrDefault(c => c.Type == "Department");
                var positionClaim = claims.FirstOrDefault(c => c.Type == "Position");

                var userRoles = await _UserManager.GetRolesAsync(user);

                return (user, departmentClaim, positionClaim, userRoles);
            }

            return (null, null, null, null);
        }
    }
}
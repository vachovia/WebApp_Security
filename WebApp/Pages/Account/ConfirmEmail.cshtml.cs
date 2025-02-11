using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Data;

namespace WebApp.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        [BindProperty]
        public string Message { get; set; } = string.Empty;
        public UserManager<AppUser> _UserManager { get; }

        public ConfirmEmailModel(UserManager<AppUser> UserManager)
        {
            _UserManager = UserManager;
        }

        public async Task<IActionResult> OnGet(string userId, string token)
        {
            var user = await _UserManager.FindByIdAsync(userId);

            if(user != null)
            {
                var result = await _UserManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    Message = "Email address is successfully confirmed, you can now try to login.";

                    return Page();
                }
            }

            Message = "Failed to validate email.";

            return Page();
        }
    }
}

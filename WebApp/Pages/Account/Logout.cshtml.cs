using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Data;

namespace WebApp.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<AppUser> _SignInManager;

        public LogoutModel(SignInManager<AppUser> SignInManager)
        {
            _SignInManager = SignInManager;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _SignInManager.SignOutAsync();

            return RedirectToPage("/Account/Login");
        }
    }
}

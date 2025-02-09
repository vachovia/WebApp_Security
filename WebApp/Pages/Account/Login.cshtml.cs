using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Data;
using WebApp.ViewModels;

namespace WebApp.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginViewModel LoginViewModel { get; set; } = new();
        private readonly SignInManager<AppUser> _SignInManager;

        public LoginModel(SignInManager<AppUser> SignInManager)
        {
            _SignInManager = SignInManager;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) { return Page(); }

            var result = await _SignInManager.PasswordSignInAsync(LoginViewModel.Email, LoginViewModel.Password, LoginViewModel.RememberMe, false);

            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }

            //if (result.RequiresTwoFactor)
            //{
            //    return RedirectToPage("/Account/LoginTwoFactor", new
            //    {
            //        Email = Credential.Email,
            //        RememberMe = Credential.RememberMe
            //    });
            //}

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("Login", "You are locked out.");
            }
            else
            {
                ModelState.AddModelError("Login", "Failed to Login.");
            }

            return Page();
        }
    }
}

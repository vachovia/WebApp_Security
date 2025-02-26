using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Data;
using WebApp.ViewModels;

namespace WebApp.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<AppUser> _SignInManager;

        [BindProperty]
        public LoginViewModel LoginViewModel { get; set; } = new();

        [BindProperty]
        public string? SuccessMessage { get; set; }

        public LoginModel(SignInManager<AppUser> SignInManager)
        {
            _SignInManager = SignInManager;
        }

        public IActionResult OnGet(string? regSuccessMessage)
        {
            SuccessMessage = regSuccessMessage;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) { return Page(); }

            var result = await _SignInManager.PasswordSignInAsync(LoginViewModel.Email, LoginViewModel.Password, LoginViewModel.RememberMe, false);

            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }
            
            if (result.RequiresTwoFactor)
            {
                /* Email 2FA*/
                //return RedirectToPage("/Account/LoginTwoFactor", new
                //{
                //    LoginViewModel.Email, LoginViewModel.RememberMe
                //});

                /* Authenticator 2FA*/
                return RedirectToPage("/Account/AuthenticatorLoginWithTwoFactor", new
                {
                    LoginViewModel.RememberMe // new syntax: RememberMe = LoginViewModel.RememberMe
                });
            }

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

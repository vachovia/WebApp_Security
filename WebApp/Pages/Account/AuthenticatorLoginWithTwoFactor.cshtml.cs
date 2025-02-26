using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Data;
using WebApp.ViewModels;

namespace WebApp.Pages.Account
{
    public class AuthenticatorLoginWithTwoFactorModel : PageModel
    {
        private readonly SignInManager<AppUser> _SignInManager;

        [BindProperty]
        public AuthenticatorMFAViewModel AuthenticatorMFA { get; set; }

        public AuthenticatorLoginWithTwoFactorModel(SignInManager<AppUser> SignInManager)
        {
            AuthenticatorMFA = new AuthenticatorMFAViewModel();
            _SignInManager = SignInManager;
        }

        public void OnGet(bool rememberMe)
        {
            AuthenticatorMFA.SecurityCode = string.Empty;
            AuthenticatorMFA.RememberMe = rememberMe;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // In cookie can be found Idsentity.TwoFactorUserId and when we submit Security Code it knows for which user
            // This page even not protected by [Authorize] attribute so why Cookie has UserId value
            var result = await _SignInManager.TwoFactorAuthenticatorSignInAsync(AuthenticatorMFA.SecurityCode, AuthenticatorMFA.RememberMe, false);
            // Last parameter is false: Flag indicating whether the current browser should be
            // remember, suppressing all further two factor authentication prompts.

            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Authenticator2FA", "You are locked out.");
                }
                else
                {
                    ModelState.AddModelError("Authenticator2FA", "Failed to login.");
                }

                return Page();
            }
        }
    }
}

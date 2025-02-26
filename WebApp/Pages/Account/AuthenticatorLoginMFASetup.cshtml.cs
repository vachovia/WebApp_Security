using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Data;
using WebApp.ViewModels;

namespace WebApp.Pages.Account
{
    [Authorize]
    public class AuthenticatorLoginMFASetupModel : PageModel
    {
        [BindProperty]
        public SetupMFAViewModel SetupMFAViewModel { get; set; }

        [BindProperty]
        public bool Succeeded { get; set; }

        private readonly UserManager<AppUser> _UserManager;

        public AuthenticatorLoginMFASetupModel(UserManager<AppUser> UserManager)
        {
            _UserManager = UserManager;
            SetupMFAViewModel = new SetupMFAViewModel();
        }

        public async Task OnGetAsync()
        {
            // To enable MFA we need to make False TwoFactorAuth in AspNetUser table cell
            var user = await _UserManager.GetUserAsync(User);

            Succeeded = false;

            if (user != null)
            {
                await _UserManager.ResetAuthenticatorKeyAsync(user);

                var key = await _UserManager.GetAuthenticatorKeyAsync(user);
                // This method inserts key into UserTokens table

                SetupMFAViewModel.Key = key ?? string.Empty;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) { return Page(); }

            var user = await _UserManager.GetUserAsync(User);

            if (user != null)
            {
                var result = await _UserManager.VerifyTwoFactorTokenAsync(
                    user, _UserManager.Options.Tokens.AuthenticatorTokenProvider, SetupMFAViewModel.SecurityCode
                );

                if (result)
                {
                    Succeeded = true;

                    await _UserManager.SetTwoFactorEnabledAsync(user, true);
                    // Since it is enabled from Login page you
                    // have to navigate to confirm 2FA from Email
                }                
            }
            else
            {
                ModelState.AddModelError("AuthenticatorSetup", "Something went wrong with the authenticator setup");
            }

            return Page();
        }
    }
}

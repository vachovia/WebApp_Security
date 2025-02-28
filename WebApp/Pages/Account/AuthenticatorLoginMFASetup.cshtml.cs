using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;
using WebApp.Data;
using WebApp.ViewModels;
using static QRCoder.PayloadGenerator;

namespace WebApp.Pages.Account
{
    public class AuthenticatorLoginMFASetupModel : PageModel
    {
        [BindProperty]
        public string? Message { get; set; } = string.Empty;
        [BindProperty]
        public SetupMFAViewModel SetupMFAViewModel { get; set; }

        private readonly UserManager<AppUser> _UserManager;

        public AuthenticatorLoginMFASetupModel(UserManager<AppUser> UserManager)
        {
            _UserManager = UserManager;
            SetupMFAViewModel = new SetupMFAViewModel();
        }

        public async Task OnGetAsync(string email, string message)
        {
            // var user = await _UserManager.GetUserAsync(User);
            var user = await _UserManager.FindByEmailAsync(email);

            Message = message;

            if (user != null)
            {
                await _UserManager.ResetAuthenticatorKeyAsync(user);

                var key = await _UserManager.GetAuthenticatorKeyAsync(user);
                // This method inserts key into UserTokens table

                SetupMFAViewModel.Email = email;
                SetupMFAViewModel.Key = key ?? string.Empty;
                SetupMFAViewModel.QRCodeBytes = GenerateQRCodeBytes("WebApp", SetupMFAViewModel.Key, user.Email ?? string.Empty);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) { return Page(); }

            // var user = await _UserManager.GetUserAsync(User);
            var user = await _UserManager.FindByEmailAsync(SetupMFAViewModel.Email ?? string.Empty);

            if (user != null)
            {
                var result = await _UserManager.VerifyTwoFactorTokenAsync(
                    user, _UserManager.Options.Tokens.AuthenticatorTokenProvider, SetupMFAViewModel.SecurityCode
                );

                if (result)
                {
                    /* This line is not needed because activated from Register page */
                    await _UserManager.SetTwoFactorEnabledAsync(user, true);

                    return RedirectToPage("/Account/Login", new
                    {
                        regSuccessMessage = "Authenticator is successfully setup."
                    });
                }                
            }
            else
            {
                ModelState.AddModelError("AuthenticatorSetup", "Something went wrong with the authenticator setup");
            }

            return Page();
        }

        private byte[] GenerateQRCodeBytes(string provider, string key, string userEmail)
        {
            var qrCodeGenerator = new QRCodeGenerator();

            var qrCodeData = qrCodeGenerator.CreateQrCode($"otpauth://totp/{provider}:{userEmail}?secret={key}&issuer={provider}",QRCodeGenerator.ECCLevel.Q);

            var qrCode = new PngByteQRCode(qrCodeData);

            return qrCode.GetGraphic(20);
        }
    }
}

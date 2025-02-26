using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using WebApp.Data;
using WebApp.Services.Interfaces;
using WebApp.ViewModels;

namespace WebApp.Pages.Account
{
    public class LoginTwoFactorModel : PageModel
    {
        public IEmailService _EmailService { get; set; }
        private readonly UserManager<AppUser> _UserManager;
        private readonly SignInManager<AppUser> _SignInManager;        

        [BindProperty]
        public EmailMFAViewModel EmailMFA { get; set; }

        public LoginTwoFactorModel(UserManager<AppUser> UserManager, SignInManager<AppUser> SignInManager, IEmailService EmailService)
        {
            _UserManager = UserManager;
            _EmailService = EmailService;
            _SignInManager = SignInManager;
            EmailMFA = new EmailMFAViewModel();
        }

        public async Task<IActionResult> OnGet(string email, bool rememberMe)
        {
            EmailMFA.SecurityCode = string.Empty;
            EmailMFA.RememberMe = rememberMe; // stored to use in OnPostAsync

            var user = await _UserManager.FindByEmailAsync(email);

            if (user != null)
            {
                var securityCode = await _UserManager.GenerateTwoFactorTokenAsync(user, "Email");

                await _EmailService.SendAsync(email, "My Web App's One Time Password (OTP)", $"Please use this code as the OTP: {securityCode}");
            }
            else
            {
                return RedirectToPage("/Account/Login");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var result = await _SignInManager.TwoFactorSignInAsync("Email", EmailMFA.SecurityCode, EmailMFA.RememberMe, false);
            // Last parameter is false: Flag indicating whether the current browser should be
            // remember, suppressing all further two factor authentication prompts.

            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("Login2FA", "You are locked out.");
            }
            else
            {
                ModelState.AddModelError("Login2FA", "Failed to login.");
            }

            return Page();
        }
    }
}

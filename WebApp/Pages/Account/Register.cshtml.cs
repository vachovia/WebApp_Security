using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using WebApp.Data;
using WebApp.Services.Interfaces;
using WebApp.Settings;
using WebApp.ViewModels;

namespace WebApp.Pages.Account
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public RegisterViewModel RegisterViewModel { get; set; } = new();

        private readonly UserManager<AppUser> _UserManager;
        public IEmailService _EmailService { get; set; }

        public RegisterModel(UserManager<AppUser> UserManager, IEmailService EmailService)
        {
            _UserManager = UserManager;
            _EmailService = EmailService;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Validate Email Address => this is optional
            // because we specified it in options.User.RequireUniqueEmail = true;

            // Create User
            var user = new AppUser
            {
                FirstName = RegisterViewModel.FirstName,
                LastName = RegisterViewModel.LastName,
                Email = RegisterViewModel.Email,
                UserName = RegisterViewModel.Email,
                Department = RegisterViewModel.Department,
                Position = RegisterViewModel.Position,
                // TwoFactorEnabled = true // If two Factor enabled then from Login page it will navigate to LoginTwoFactor
            };

            var userClaims = new List<Claim>()
            {
                new Claim(SD.Position, RegisterViewModel.Position),
                new Claim(SD.Department, RegisterViewModel.Department)
            };

            // Generates user Id needed in GenerateEmailConfirmationTokenAsync
            var result = await _UserManager.CreateAsync(user, RegisterViewModel.Password);

            if (result.Succeeded)
            {
                await _UserManager.AddClaimsAsync(user, userClaims);

                await _UserManager.AddToRolesAsync(user, new List<string> { SD.EmployeeRole });

                /* adjust Program.cs and add AddDefaultTokenProviders() to have token */
                var confirmationToken = await _UserManager.GenerateEmailConfirmationTokenAsync(user);

                // Dry run: Use this logic to Confirm Email without sending email
                // return Redirect(Url.PageLink(pageName: "/Account/ConfirmEmail", values: new { userId = user.Id, token = confirmationToken }) ?? "");

                var confirmationLink = Url.PageLink(pageName: "/Account/ConfirmEmail", values: new { userId = user.Id, token = confirmationToken });

                await _EmailService.SendAsync(user.Email, "Please confirm your email", $"Please click on this link to confirm your email address: {confirmationLink}");

                return RedirectToPage("/Account/Login", new
                {
                    regSuccessMessage = "Confirmation email was sent, please confirm your email."
                });
            }
                
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError("Register", error.Description);
            }

            return Page();
        }
    }
}

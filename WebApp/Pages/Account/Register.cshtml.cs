using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using WebApp.Data;
using WebApp.Services.Interfaces;
using WebApp.ViewModels;

namespace WebApp.Pages.Account
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public RegisterViewModel RegisterViewModel { get; set; } = new();

        private readonly UserManager<AppUser> _UserManager;
        public IEmailService _emailService { get; set; }

        public RegisterModel(UserManager<AppUser> UserManager, IEmailService emailService)
        {
            _UserManager = UserManager;
            _emailService = emailService;
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
                Position= RegisterViewModel.Position
            };

            var claimDepartment = new Claim("Department", RegisterViewModel.Department);
            var claimPosition = new Claim("Position", RegisterViewModel.Position);

            // Generates user Id needed in GenerateEmailConfirmationTokenAsync
            var result = await _UserManager.CreateAsync(user, RegisterViewModel.Password);

            if (result.Succeeded)
            {
                await _UserManager.AddClaimAsync(user, claimDepartment);
                await _UserManager.AddClaimAsync(user, claimPosition);

                /* adjust Program.cs and add AddDefaultTokenProviders() to have token */
                var confirmationToken = await _UserManager.GenerateEmailConfirmationTokenAsync(user);

                // Dry run: Use this logic to Confirm Email without sending email
                // return Redirect(Url.PageLink(pageName: "/Account/ConfirmEmail", values: new { userId = user.Id, token = confirmationToken }) ?? "");

                var confirmationLink = Url.PageLink(pageName: "/Account/ConfirmEmail", values: new { userId = user.Id, token = confirmationToken });

                await _emailService.SendAsync(user.Email, "Please confirm your email", $"Please click on this link to confirm your email address: {confirmationLink}");

                return RedirectToPage("/Account/Login");
            }
                
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError("Register", error.Description);
            }

            return Page();
        }
    }
}

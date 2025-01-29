using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using WebApp_UnderTheHood.Models.AuthModels;

namespace WebApp_UnderTheHood.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential Credential { get; set; } = new();

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid) return Page();

            if(Credential.UserName == "admin" && Credential.Password == "password")
            {
                // Creating security context
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "admin@mywebsite.com"),
                    new Claim("Department", "HR"),
                    new Claim("Admin", "true"), // value doesn't matter
                    new Claim("Manager", "true"), // value doesn't matter
                    new Claim("EmploymentDate", "2024-07-01")
                };
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");

                var claimsPrincipal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = Credential.RememberMe
                };

                await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal, authProperties);
                // No sign-in authentication handlers are registered (IAuthenticationService).
                // Did you forget to call AddAuthentication().AddCookie("MyCookieAuth",...)?

                return RedirectToPage("/Index");
            }
            return Page();
        }
    }
}

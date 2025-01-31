using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp_UnderTheHood.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}


// As soon as we use [Authorize] attribute on class declaration
// app navigates to Login page by default /Account/Login
// if we change folder Name Account to any other it will not navigate
// otherwise we need to specify in Program.cs in AddCookie like
// options.LoginPath = "/Account/Hello"
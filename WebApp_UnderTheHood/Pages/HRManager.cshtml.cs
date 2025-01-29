using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using WebApp_UnderTheHood.Models;
using WebApp_UnderTheHood.Models.AuthModels;

namespace WebApp_UnderTheHood.Pages
{
    [Authorize(Policy ="HRManagerOnly")]
    public class HRManagerModel : PageModel
    {
        [BindProperty]
        public List<WeatherForecast> weatherForecastsItems { get; set; } = new();

        private readonly IHttpClientFactory _httpClientFactory;

        public HRManagerModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public void OnGet()
        {

        }

        //public async Task OnGetAsync()
        //{
        //    JwtToken token;

        //    var strTokenObj = HttpContext.Session.GetString("access_token");

        //    if (string.IsNullOrEmpty(strTokenObj))
        //    {
        //        token = await Authenticate();
        //    }
        //    else
        //    {
        //        token = JsonConvert.DeserializeObject<JwtToken>(strTokenObj) ?? new JwtToken();
        //    }

        //    if (token == null || string.IsNullOrWhiteSpace(token.AccessToken) || token.ExpiresAt <= DateTime.UtcNow)
        //    {
        //        token = await Authenticate();
        //    }

        //    var httpClient = _httpClientFactory.CreateClient("OurWebAPI");

        //    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token?.AccessToken ?? string.Empty);

        //    weatherForecastsItems = await httpClient.GetFromJsonAsync<List<WeatherForecast>>("WeatherForecast") ?? new(); // controller name
        //}

        private async Task<JwtToken> Authenticate()
        {
            var httpClient = _httpClientFactory.CreateClient("OurWebAPI");

            var res = await httpClient.PostAsJsonAsync("Auth", new { UserName = "admin", Password = "password" }); // We already specified BaseAddress so need only Auth action name

            res.EnsureSuccessStatusCode();

            string strJwt = await res.Content.ReadAsStringAsync();

            HttpContext.Session.SetString("access_token", strJwt);

            var token = JsonConvert.DeserializeObject<JwtToken>(strJwt)?? new JwtToken();

            return token;
        }
    }
}

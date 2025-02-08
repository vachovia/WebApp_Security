using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Need these two packages:
        // Microsoft.AspNetCore.Authentication.JwtBearer & System.IdentityModel.Tokens.Jwt

        [HttpPost]
        public IActionResult Authenticate([FromBody] Credential credential)
        {
            if (credential.UserName == "admin" && credential.Password == "password")
            {
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "admin@mywebsite.com"),
                    new Claim("Department", "HR"),
                    new Claim("Admin", "true"), // value doesn't matter
                    new Claim("Manager", "true"), // value doesn't matter
                    new Claim("EmploymentDate", "2024-07-01")
                };

                var expiresAt = DateTime.UtcNow.AddMinutes(20);
                var accessToken = CreateToken(claims, expiresAt);

                return Ok(new {
                    access_token = accessToken,
                    expires_at = expiresAt
                });
            }

            ModelState.AddModelError("Unauthorized", "You are not authorized to access the endpoint");

            return Unauthorized(ModelState);
        }

        private string CreateToken(IEnumerable<Claim> claims, DateTime expireAt)
        {
            // Added Microsoft.AspNetCore.Authentication.JwtBearer and System.IdentityModel.Tokens.Jwt

            var secretKey = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("SecretKey")??"");
            var signinCreds = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);
            
            var jwt = new JwtSecurityToken(
                claims: claims,
                expires: expireAt,
                notBefore: DateTime.UtcNow,
                signingCredentials: signinCreds
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }

    public class Credential
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

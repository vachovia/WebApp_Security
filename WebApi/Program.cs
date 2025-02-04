using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var secretKey = builder.Configuration.GetValue<string>("SecretKey");
var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey ?? string.Empty));

// Can be simply builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => { ... });
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = symmetricSecurityKey,        
        ValidateAudience = false,
        ValidateIssuer = false,

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// If [Authorize] then don't need below middleware otherwise with policy add
builder.Services.AddAuthorization(options =>
{
    // this "Admin" claim is in AuthController Authenticate action
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// added to handle [Auithorization]
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

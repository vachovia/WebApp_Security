using Microsoft.AspNetCore.Authorization;
using WebApp_UnderTheHood.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options =>
{
    options.Cookie.Name = "MyCookieAuth";
    options.LoginPath = "/Account/Login"; // this is not need to be specified it is default path
    options.AccessDeniedPath = "/Account/AccessDenied"; // this is not need to be specified it is default path
    options.ExpireTimeSpan = TimeSpan.FromSeconds(30); // cookie expires and we kicked out or browser is closed if we didn't specify persistent cookie
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustBelongToHRDepartment", policy => policy.RequireClaim("Department", "HR"));
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin"));
    options.AddPolicy("HRManagerOnly", policy => policy.RequireClaim("Department", "HR").RequireClaim("Manager").Requirements.Add(new HRManagerProbationRequirement(3)));
});

builder.Services.AddSingleton<IAuthorizationHandler, HRManagerProbationRequirementHandler>();

// to store token in session
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true; // cookie accessable only from http not by any javascript
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromSeconds(20);
});

builder.Services.AddHttpClient("OurWebAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7111/");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // auth middleware
app.UseAuthorization(); // auz middleware for [Authorize]

app.UseSession();

app.MapRazorPages();

app.Run();

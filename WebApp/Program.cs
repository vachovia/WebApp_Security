using WebApp.Data;
using WebApp.Services;
using WebApp.Settings;
using WebApp.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// CSS isolation is not working with RazorRuntimeCompilation.
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// AppUser is the class that extends IdentityUser
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.User.RequireUniqueEmail = true;

    options.SignIn.RequireConfirmedEmail = true;

}).AddEntityFrameworkStores<AppDbContext>() // AppDbContext is InMemory represetation of DB
.AddDefaultTokenProviders(); // this provides Tokens for Email Confirmation and 2FA/MFA

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
});

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SMTP"));
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddScoped<IContextSeedService, ContextSeedService>();

builder.Services.AddAuthorization(opt => {
    opt.AddPolicy(SD.AdminPolicy, policy => policy.RequireRole(SD.AdminRole));
    opt.AddPolicy(SD.ManagerPolicy, policy => policy.RequireRole(SD.ManagerRole));
    opt.AddPolicy(SD.AdminOrManagerPolicy, policy => policy.RequireRole(SD.AdminRole, SD.ManagerRole));
    opt.AddPolicy(SD.AdminAndManagerPolicy, policy => policy.RequireRole(SD.AdminRole).RequireRole(SD.ManagerRole));
    opt.AddPolicy(SD.SuperAdminPolicy, policy => policy.RequireAssertion(context => SD.SuperAdminPolicyCheck(context)));
});

builder.Services.AddCors();

var app = builder.Build();

app.UseCors(options =>
{
    options.AllowAnyMethod().AllowAnyHeader().AllowCredentials().WithExposedHeaders("*"); // .WithOrigins("*").
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

#region Context Seed
using var scope = app.Services.CreateScope();
try
{
    // NuGet Package Manager Console -> Drop-Database
    // Needed: builder.Services.AddScoped<IContextSeedService, ContextSeedService>();
    var contextSeedService = scope.ServiceProvider.GetService<IContextSeedService>();
    await contextSeedService!.InitializeContextAsync();
}
catch (Exception ex)
{
    var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
    logger!.LogError(ex.Message, "Failed to initialize and seed the database.");
}
#endregion

app.Run();

// Drop-Database
// Update-Database
// Add-Migration AddingUserToDatabase -o Data/Migrations
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApp.Data;
using WebApp.Services.Interfaces;
using WebApp.Settings;
using WebApp.ViewModels;

namespace WebApp.Services
{
    public class ContextSeedService : IContextSeedService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ContextSeedService(AppDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task InitializeContextAsync()
        {
            var pendingCount = _context.Database.GetPendingMigrationsAsync().GetAwaiter().GetResult().Count();

            if (pendingCount > 0)
            {
                // Applies for any pending migration
                await _context.Database.MigrateAsync();
            }

            var anyRole = _roleManager.Roles.Any();
            // var anyRole = _roleManager.Roles.AnyAsync().GetAwaiter().GetResult();

            if (!anyRole)
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = SD.AdminRole
                });
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = SD.ManagerRole
                });
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = SD.EmployeeRole
                });
            }

            var anyUser = _userManager.Users.AnyAsync().GetAwaiter().GetResult();

            if (!anyUser)
            {
                var admin = new AppUser
                {
                    FirstName = "Admin",
                    LastName = "Jackson",
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    Position = SD.SecurityAdminPosition,
                    Department = SD.SecurityDepartment,
                    EmailConfirmed = true,
                };

                await _userManager.CreateAsync(admin, "P@ssword1");

                await _userManager.AddToRolesAsync(admin, [
                    SD.AdminRole,
                    SD.ManagerRole,
                    SD.EmployeeRole
                ]);

                await _userManager.AddClaimsAsync(admin,
                [
                    new Claim(SD.Department, SD.SecurityDepartment),
                    new Claim(SD.Position, SD.SecurityAdminPosition)
                ]);

                var manager = new AppUser
                {
                    FirstName = "Manager",
                    LastName = "Wilson",
                    UserName = "manager@example.com",
                    Email = "manager@example.com",
                    Position = SD.ManagerPosition,
                    Department = SD.HRDepartment,
                    EmailConfirmed = true,
                };

                await _userManager.CreateAsync(manager, "P@ssword1");

                await _userManager.AddToRoleAsync(manager, SD.ManagerRole);

                await _userManager.AddClaimsAsync(manager,
                [
                    new Claim(SD.Department, SD.HRDepartment),
                    new Claim(SD.Position, SD.ManagerPosition)
                ]);

                var developer = new AppUser
                {
                    FirstName = "Developer",
                    LastName = "Miller",
                    UserName = "dev@example.com",
                    Email = "dev@example.com",
                    Position = SD.DeveloperPosition,
                    Department = SD.ITDepartment,
                    EmailConfirmed = true,
                };

                await _userManager.CreateAsync(developer, "P@ssword1");

                await _userManager.AddToRoleAsync(developer, SD.EmployeeRole);

                await _userManager.AddClaimsAsync(developer,
                [
                    new Claim(SD.Department, SD.ITDepartment),
                    new Claim(SD.Position, SD.DeveloperPosition)
                ]);
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WebApp.Settings
{
    public static class SD
    {
        public const string Position = "Position";
        public const string Department = "Department";
        public const string HRDepartment = "HR";
        public const string ITDepartment = "IT";
        public const string AdminPosition = "Admin";
        public const string ManagerPosition = "Manager";
        public const string DeveloperPosition = "Developer";
        public const string SecurityAdminRole = "SecurityAdmin";
        public const string SecurityAdminPosition = "SecurityAdmin";
        public const string SecurityDepartment = "Security";       
        public const string AdminRole = "Admin";
        public const string ManagerRole = "Manager";
        public const string EmployeeRole = "Employee";
        public const string AdminPolicy = "AdminPolicy";
        public const string ManagerPolicy = "ManagerPolicy";
        public const string SuperAdminPolicy = "SuperAdminPolicy";
        public const string AdminOrManagerPolicy = "AdminOrManagerPolicy";
        public const string AdminAndManagerPolicy = "AdminAndManagerPolicy";

        public static bool SuperAdminPolicyCheck(AuthorizationHandlerContext context)
        {
            var isVip = context.User.IsInRole(AdminRole) && context.User.HasClaim(c => c.Type == ClaimTypes.Email && c.Value.Contains("admin"));

            return isVip;
        }
    }
}

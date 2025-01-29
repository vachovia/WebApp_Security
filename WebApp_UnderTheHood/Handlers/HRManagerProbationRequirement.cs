using Microsoft.AspNetCore.Authorization;

namespace WebApp_UnderTheHood.Handlers
{
    public class HRManagerProbationRequirement: IAuthorizationRequirement
    {
        public int ProbationMonths { get; }

        public HRManagerProbationRequirement(int probationMonths)
        {
            ProbationMonths = probationMonths;
        }
    }

    public class HRManagerProbationRequirementHandler : AuthorizationHandler<HRManagerProbationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HRManagerProbationRequirement requirement)
        {
            if(!context.User.HasClaim(x => x.Type == "EmploymentDate")) {
                return Task.CompletedTask;
            }

            var empDateRaw = context.User.FindFirst(x => x.Type == "EmploymentDate")?.Value;

            if(DateTime.TryParse(empDateRaw, out DateTime empDate))
            {
                var period = DateTime.Now - empDate;

                if (period.Days > 30 * requirement.ProbationMonths)
                {
                    context.Succeed(requirement);
                }
            }           

            return Task.CompletedTask;
        }
    }
}

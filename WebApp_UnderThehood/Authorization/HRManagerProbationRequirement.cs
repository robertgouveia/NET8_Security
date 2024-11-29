using Microsoft.AspNetCore.Authorization;

namespace WebApp_UnderThehood.Authorization;

public class HrManagerProbationRequirement : IAuthorizationRequirement
{
    public HrManagerProbationRequirement(int probationLengthInMonths)
    {
        ProbationLengthInMonths = probationLengthInMonths;
    }
    
    public int ProbationLengthInMonths { get; }
}

public class HRManagerProbationRequirementHandler : AuthorizationHandler<HrManagerProbationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HrManagerProbationRequirement requirement)
    {
        // Unsuccessful
        if (!context.User.HasClaim(x => x.Type == "EmploymentDate")) return Task.CompletedTask;

        if (!DateTime.TryParse(context.User.FindFirst(x => x.Type == "EmploymentDate")?.Value, out var employmentDate)) return Task.CompletedTask;
        
        var period = DateTime.Now - employmentDate;
        
        // 30 * months = (probation in days)
        if (period.Days > 30 * requirement.ProbationLengthInMonths) context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
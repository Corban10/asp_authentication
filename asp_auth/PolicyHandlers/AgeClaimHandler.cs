using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace asp_auth.PolicyHandlers
{
    public class AgeClaimRequirement : IAuthorizationRequirement
    {
        public int MinimumAge { get; }
        public int MaximumAge { get; }
        public AgeClaimRequirement(int minimumAge, int maximumAge)
        {
            MinimumAge = minimumAge;
            MaximumAge = maximumAge;
        }
    }
    public class AgeClaimHandler : AuthorizationHandler<AgeClaimRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AgeClaimRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.DateOfBirth))
            {
                return Task.CompletedTask;
            }

            var dateOfBirth = Convert.ToDateTime(context.User.FindFirst(c => c.Type == ClaimTypes.DateOfBirth).Value);

            int calculatedAge = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth > DateTime.Today.AddYears(-calculatedAge))
            {
                calculatedAge--;
            }

            if (calculatedAge >= requirement.MinimumAge && calculatedAge <= requirement.MaximumAge)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}

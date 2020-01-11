using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Threading.Tasks;

namespace taohi_backend.PolicyHandlers
{
    public class CustomClaimRequirement : IAuthorizationRequirement
    {
        public string Value { get; set; }
        public CustomClaimRequirement(string value)
        {
            Value = value;
        }
    }
    public class CustomClaimHandler : AuthorizationHandler<CustomClaimRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomClaimRequirement requirement)
        {
            if (context.User.HasClaim("ArbitraryString", requirement.Value))
                context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Threading.Tasks;

namespace taohi_backend.PolicyHandlers
{
    public class CustomClaimOperations : IAuthorizationRequirement
    {
        public string Value { get; set; }
        public CustomClaimOperations(string type, string value)
        {
            Value = value;
        }
    }
    public class CustomClaimAuthHandler : AuthorizationHandler<CustomClaimOperations>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CustomClaimOperations requirement)
        {
            if (context.User.HasClaim("ArbitraryString", requirement.Value))
                context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}

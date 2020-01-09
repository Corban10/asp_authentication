using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Threading.Tasks;

namespace taohi_backend.PolicyHandlers
{
    public class CustomClaimOperations : IAuthorizationRequirement
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public CustomClaimOperations(string type, string value)
        {
            Type = type;
            Value = value;
        }
    }
    public class CustomClaimAuthHandler : AuthorizationHandler<CustomClaimOperations>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CustomClaimOperations requirement)
        {
            if (context.User.HasClaim(requirement.Type, requirement.Value))
                context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}

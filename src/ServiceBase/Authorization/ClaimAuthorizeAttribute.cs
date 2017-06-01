using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceBase.Authorization
{
    public class ScopeAuthorizeAttribute : AuthorizeAttribute
    {
        public string Issuer { get; set; }
        public string Scope {  get { return this.Policy; } }

        public ScopeAuthorizeAttribute(string scope) : base(scope)
        {
            
        }

        public ScopeAuthorizeAttribute(string scope, string issuer) : base(scope)
        {
            this.Issuer = issuer; 
        }
    }
    
    public static class ScopeAuthorizeIServiceCollectionExtensions
    {
        public static IServiceCollection AddScopeAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                //options.AddScopePolicy("useraccount:read", issuer);
                //options.AddScopePolicy("useraccount:write", issuer);
                //options.AddScopePolicy("useraccount:delete", issuer);
            });

            return services;
        }


        private static void AddScopePolicy(this AuthorizationOptions options, string scope, string issuer)
        {
            options.AddPolicy(scope, policy => policy.Requirements.Add(new HasScopeRequirement(scope, issuer)));
        }
    }

    public class HasScopeRequirement : AuthorizationHandler<HasScopeRequirement>, IAuthorizationRequirement
    {
        private readonly string issuer;
        private readonly string scope;

        public HasScopeRequirement(string scope, string issuer)
        {
            this.scope = scope;
            this.issuer = issuer;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
        {
            // If user does not have the scope claim, get out of here
            if (!context.User.HasClaim(c => c.Type == "scope" && c.Issuer == issuer))
                return Task.CompletedTask;

            // Split the scopes string into an array
            //var scopes = context.User.FindFirst(c => c.Type == "scope" && c.Issuer == issuer).Value.Split(' ');
            var scopes = context.User.FindAll(c => c.Type == "scope" && c.Issuer == issuer).Select(s => s.Value);

            // Succeed if the scope array contains the required scope
            if (scopes.Any(s => s == scope))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}

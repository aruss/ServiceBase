namespace ServiceBase.Authorization
{
    using Microsoft.AspNetCore.Authorization;

    public class ScopeAuthorizeAttribute : AuthorizeAttribute
    {
        public string Scope
        {
            get
            {
                return this.Policy;
            }
        }

        public ScopeAuthorizeAttribute(
            string scope,
            // bearer is most of the times if you use scoped authentication
            string schemes = "Bearer") 
            : base(scope)
        {
            this.AuthenticationSchemes = "Bearer"; 
        }
    }
}
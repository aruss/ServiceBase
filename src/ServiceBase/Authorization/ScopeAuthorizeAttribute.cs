using Microsoft.AspNetCore.Authorization;

namespace ServiceBase.Authorization
{
    public class ScopeAuthorizeAttribute : AuthorizeAttribute
    {
        public string Scope {  get { return this.Policy; } }

        public ScopeAuthorizeAttribute(string scope) : base(scope)
        {
            
        }
    }
}

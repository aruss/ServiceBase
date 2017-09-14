namespace ServiceBase.Authorization
{
    using Microsoft.AspNetCore.Authorization;

    public class ScopeAuthorizeAttribute : AuthorizeAttribute
    {
        public string Scope {  get { return this.Policy; } }

        public ScopeAuthorizeAttribute(string scope) : base(scope)
        {
            
        }
    }
}

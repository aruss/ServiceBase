using Microsoft.AspNetCore.Authorization;

namespace ServiceBase.Authorization
{
    public class ScopeAuthorizeAttribute : AuthorizeAttribute
    {
        //public string Issuer { get; set; }
        public string Scope {  get { return this.Policy; } }

        public ScopeAuthorizeAttribute(string scope) : base(scope)
        {
            
        }

        /*public ScopeAuthorizeAttribute(string scope, string issuer) : base(scope)
        {
            this.Issuer = issuer; 
        }*/
    }
}

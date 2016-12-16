using System;

namespace ServiceBase.IdentityServer.Events
{
    public class UserAccountCreatedDetails
    {
        public Guid UserAccountId { get; set; }
        public string Provider { get; set; }
    }
}

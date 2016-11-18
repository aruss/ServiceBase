using System;

namespace ServiceBase.IdentityServer.EntityFramework.Entities
{
    public class ExternalAccount
    {
        public Guid UserId { get; set; }
        public string Provider { get; set; }
        public string Subject { get; set; }
        public string Email { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserAccount UserAccount { get; set; }
    }
}

using System;

namespace ServiceBase.IdentityServer.Models
{
    public class UserClaim
    {
        public UserClaim(string type, string value, string valueType = null)
        {
            this.Type = type;
            this.Value = value;
            this.ValueType = valueType;
        }

        public Guid UserId { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string ValueType { get; set; }
    }
}

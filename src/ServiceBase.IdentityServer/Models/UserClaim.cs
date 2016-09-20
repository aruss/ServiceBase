using System;

namespace Host.Models
{
    public class UserClaim
    {
        public Guid UserId { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string ValueType { get; set; }
    }
}

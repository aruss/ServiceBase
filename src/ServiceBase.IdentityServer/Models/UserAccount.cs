using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ServiceBase.IdentityServer.Models
{
    public enum VerificationKeyPurpose
    {
        ResetPassword = 1,
        ChangeEmail = 2,
        ChangeMobile = 3,
        ConfirmAccount = 4
    }

    public class UserAccount
    {
        public Guid Id { get; set; }

        [StringLength(50)]
        public string Tenant { get; set; }

        // Maximum length of a valid email address is 254 characters.
        // See Dominic Sayers answer at SO: http://stackoverflow.com/a/574698/99240
        [EmailAddress]
        [StringLength(254)]
        public string Email { get; set; }
        public bool IsEmailVerified { get; set; }
        public DateTime? EmailVerifiedAt { get; set; }
        
        public bool IsLoginAllowed { get; set; }
        public virtual DateTime? LastLoginAt { get; set; }
        public virtual DateTime? LastFailedLoginAt { get; set; }
        public virtual int FailedLoginCount { get;set; }
        
        [StringLength(200)]
        public string PasswordHash { get; set; }
        public DateTime? PasswordChangedAt { get; set; }

        [StringLength(100)]
        public virtual string VerificationKey { get; set; }
        public virtual int? VerificationPurpose { get; set; }
        public virtual DateTime? VerificationKeySentAt { get; set; }
        [StringLength(2000)]
        public virtual string VerificationStorage { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public IEnumerable<ExternalAccount> Accounts { get; set; }
        public IEnumerable<UserClaim> Claims { get; set; }
    }
}

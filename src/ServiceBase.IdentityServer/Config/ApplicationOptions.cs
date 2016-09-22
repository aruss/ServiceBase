namespace ServiceBase.IdentityServer.Config
{
    public class ApplicationOptions
    {
        public bool RequireAccountVerification { get; set; } = true;
        public bool LoginAfterAccountCreation { get; set; } = true;
        public bool LoginAfterAccountConfirmation { get; set; } = true;
        public bool LoginAfterAccountRecovery { get; set; } = true;
        public bool AllowAccountDeletion { get; set; } = true;
        public int VerificationKeyLifetime { get; set; } = 86400; // 24 hours
        public int AccountLockoutFailedLoginAttempts { get; set; } = 5;
        public int AccountLockoutDuration { get; set; } = 600; // 10 minutes
        public int PasswordHashingIterationCount { get; set; } = 0;
        public bool MergeAccountsAutomatically { get; set; } = true;
        public bool DisplayLoginHints { get; set; } = false;
    }
}

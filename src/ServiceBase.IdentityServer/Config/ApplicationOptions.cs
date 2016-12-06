namespace ServiceBase.IdentityServer.Config
{
    public class ApplicationOptions
    {
        // Local account options

        /// <summary>
        /// If enbaled users may login and register with local account by using email and password
        /// </summary>
        public bool LocalAccountEnabled { get; set; } = true;

        public int PasswordHashingIterationCount { get; set; } = 0;
        public int AccountLockoutFailedLoginAttempts { get; set; } = 5;
        public int AccountLockoutDuration { get; set; } = 600; // 10 minutes

        /// <summary>
        /// If enabled users have to confirm the registration by clicking in the link in the mail in order to login.
        /// </summary>
        public bool RequireLocalAccountVerification { get; set; } = true;

        // External account options

        /// <summary>
        /// If enabled users have to confirm the registration if the come from third party IDP
        /// </summary>
        public bool RequireExternalAccountVerification { get; set; } = false;

        // Common options

        /// <summary>
        /// If enabled user may delete his own account
        /// </summary>
        public bool AllowAccountDeletion { get; set; } = true;

        /// <summary>
        /// If user has trouble with login IdSrv will show all possible accounts which are
        /// </summary>
        public bool DisplayLoginHints { get; set; } = false;

        public bool LoginAfterAccountCreation { get; set; } = true;
        public bool LoginAfterAccountConfirmation { get; set; } = true;
        public bool LoginAfterAccountRecovery { get; set; } = true;

        public int VerificationKeyLifetime { get; set; } = 86400; // 24 hours

        public bool MergeAccountsAutomatically { get; set; } = true;

    }
}

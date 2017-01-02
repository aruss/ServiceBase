namespace ServiceBase.IdentityServer.Configuration
{
    /// <summary>
    /// IdentityBase application options
    /// </summary>
    public class ApplicationOptions
    {
        // Local account options

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

        /// <summary>
        /// Login user automatically after he created a local account
        /// </summary>
        public bool LoginAfterAccountCreation { get; set; } = true;

        /// <summary>
        /// Login user automatically after account confirmation
        /// </summary>
        public bool LoginAfterAccountConfirmation { get; set; } = true;

        /// <summary>
        /// Login user automatically after successful recovery
        /// </summary>
        public bool LoginAfterAccountRecovery { get; set; } = true;

        /// <summary>
        /// Timespan the confirmation and concelation links a valid in minutes
        /// </summary>
        public int VerificationKeyLifetime { get; set; } = 1440; // 24 hours

        /// <summary>
        /// Automatically merges third party accounts with local account if email matches
        /// </summary>
        public bool MergeAccountsAutomatically { get; set; } = true;
    }
}

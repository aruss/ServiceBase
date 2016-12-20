using IdentityServer4;
using IdentityServer4.Services;
using Microsoft.Extensions.Options;
using ServiceBase.IdentityServer.Config;
using ServiceBase.IdentityServer.Crypto;
using ServiceBase.IdentityServer.Events;
using ServiceBase.IdentityServer.Extensions;
using ServiceBase.IdentityServer.Models;
using System;
using System.Threading.Tasks;

namespace ServiceBase.IdentityServer.Services
{
    public class UserAccountService
    {
        ApplicationOptions _applicationOptions;
        ICrypto _crypto;
        IUserAccountStore _userAccountStore;
        IEventService _eventService;

        public UserAccountService(
            IOptions<ApplicationOptions> applicationOptions,
            ICrypto crypto,
            IUserAccountStore userAccountStore,
            IEventService eventService)
        {
            _applicationOptions = applicationOptions.Value;
            _crypto = crypto;
            _userAccountStore = userAccountStore;
            _eventService = eventService;
        }

        public async Task<UserAccount> CreateNewLocalUserAccount(
            string email,
            string password,
            string returnUrl = null)
        {
            var now = DateTime.UtcNow;

            var userAccount = new UserAccount
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = _crypto.HashPassword(password,
                    _applicationOptions.PasswordHashingIterationCount),
                FailedLoginCount = 0,
                IsEmailVerified = false,
                IsLoginAllowed = _applicationOptions.LoginAfterAccountCreation,
                PasswordChangedAt = now,
                CreatedAt = now,
                UpdatedAt = now
            };

            if (_applicationOptions.RequireLocalAccountVerification &&
                !String.IsNullOrWhiteSpace(returnUrl))
            {
                this.SetConfirmationVirificationKey(userAccount, returnUrl);
            }

            await _userAccountStore.WriteAsync(userAccount);

            // Emit event
            await _eventService.RaiseSuccessfulUserAccountCreatedEventAsync(
                userAccount.Id,
                IdentityServerConstants.LocalIdentityProvider);

            return userAccount;
        }

        public async Task SetAsVerified(UserAccount userAccount)
        {
            var now = DateTime.UtcNow;
            userAccount.IsLoginAllowed = true;
            userAccount.IsEmailVerified = true;
            userAccount.EmailVerifiedAt = now;
            userAccount.UpdatedAt = now;
            userAccount.ClearVerification();

            // Update user account
            await _userAccountStore.WriteAsync(userAccount);

            // Emit event
            await _eventService.RaiseSuccessfulUserAccountUpdatedEventAsync(
                userAccount.Id);
        }

        public void SetConfirmationVirificationKey(
            UserAccount userAccount,
            string returnUrl,
            DateTime? now = null)
        {
            // Set verification key
            userAccount.SetVerification(
                _crypto.Hash(_crypto.GenerateSalt()).StripUglyBase64(),
                VerificationKeyPurpose.ConfirmAccount,
                returnUrl,
                now);

        }
    }
}

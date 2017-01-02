﻿using IdentityServer4;
using IdentityServer4.Services;
using Microsoft.Extensions.Options;
using ServiceBase.IdentityServer.Configuration;
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
        private ApplicationOptions _applicationOptions;
        private ICrypto _crypto;
        private IUserAccountStore _userAccountStore;
        private IEventService _eventService;

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

        public async Task<bool> VerifyPasswordAsyc(UserAccount userAccount, string password)
        {
            var result = _crypto.VerifyPasswordHash(userAccount.PasswordHash, password,
                _applicationOptions.PasswordHashingIterationCount);

            var now = DateTime.UtcNow;

            if (result)
            {
                userAccount.FailedLoginCount = 0;
                userAccount.LastFailedLoginAt = null;
                userAccount.LastLoginAt = now;
            }
            else
            {
                userAccount.FailedLoginCount++;
                userAccount.LastFailedLoginAt = now;
                if (userAccount.FailedLoginCount >= _applicationOptions.AccountLockoutFailedLoginAttempts)
                {
                    userAccount.IsLoginAllowed = false;
                }
            }

            // Update user account
            userAccount.UpdatedAt = now;
            await _userAccountStore.WriteAsync(userAccount);

            return result;
        }


        public async Task<UserAccount> CreateNewLocalUserAccountAsync(
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
                IsLoginAllowed = _applicationOptions.RequireLocalAccountVerification,
                PasswordChangedAt = now,
                CreatedAt = now,
                UpdatedAt = now
            };

            if (_applicationOptions.RequireLocalAccountVerification &&
                !String.IsNullOrWhiteSpace(returnUrl))
            {
                this.SetConfirmAccountVirificationKey(userAccount, returnUrl);
            }

            await _userAccountStore.WriteAsync(userAccount);

            // Emit event
            await _eventService.RaiseSuccessfulUserAccountCreatedEventAsync(
                userAccount.Id,
                IdentityServerConstants.LocalIdentityProvider);

            return userAccount;
        }

        public async Task SetAccountRecoverAsync(UserAccount userAccount, string returnUrl = null)
        {
            userAccount.VerificationKey = _crypto.Hash(_crypto.GenerateSalt()).StripUglyBase64();
            userAccount.VerificationPurpose = (int)VerificationKeyPurpose.ResetPassword;
            userAccount.VerificationKeySentAt = DateTime.UtcNow;
            userAccount.VerificationStorage = returnUrl;

            // Update user account
            await _userAccountStore.WriteAsync(userAccount);

            // Emit event
            await _eventService.RaiseSuccessfulUserAccountUpdatedEventAsync(
                userAccount.Id);
        }

        public async Task SetEmailVerifiedAsync(UserAccount userAccount)
        {
            var now = DateTime.UtcNow;
            userAccount.IsLoginAllowed = true;
            userAccount.IsEmailVerified = true;
            userAccount.EmailVerifiedAt = now;
            userAccount.UpdatedAt = now;

            await ClearVerificationKeyAsync(userAccount);
        }

        public async Task ClearVerificationKeyAsync(UserAccount userAccount)
        {
            userAccount.ClearVerification();

            // Update user account
            await _userAccountStore.WriteAsync(userAccount);

            // Emit event
            await _eventService.RaiseSuccessfulUserAccountUpdatedEventAsync(
                userAccount.Id);
        }

        public void SetConfirmAccountVirificationKey(
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

        public void SetResetPasswordVirificationKey(
          UserAccount userAccount,
          string returnUrl,
          DateTime? now = null)
        {
            // Set verification key
            userAccount.SetVerification(
                _crypto.Hash(_crypto.GenerateSalt()).StripUglyBase64(),
                VerificationKeyPurpose.ResetPassword,
                returnUrl,
                now);
        }

        /// <summary>
        /// Validate if verification key is valid, if yes it will load a corresponding user account
        /// </summary>
        /// <param name="key"></param>
        /// <param name="purpose"></param>
        /// <returns></returns>
        public async Task<VerificationResult> HandleVerificationKey(
            string key,
            VerificationKeyPurpose purpose)
        {
            var result = new VerificationResult();
            var userAccount = await _userAccountStore.LoadByVerificationKeyAsync(key);
            if (userAccount == null)
            {
                return result;
            }

            result.UserAccount = userAccount;
            result.PurposeValid = userAccount.VerificationPurpose == (int)purpose;

            if (userAccount.VerificationKeySentAt.HasValue)
            {
                var validTill = userAccount.VerificationKeySentAt.Value +
                    TimeSpan.FromMinutes(_applicationOptions.VerificationKeyLifetime);
                var now = DateTime.Now;

                result.TokenExpired = now > validTill;
            }

            return result;
        }
    }

    public class VerificationResult
    {
        public UserAccount UserAccount { get; set; }
        public bool TokenExpired { get; set; }
        public bool PurposeValid { get; set; }
    }
}
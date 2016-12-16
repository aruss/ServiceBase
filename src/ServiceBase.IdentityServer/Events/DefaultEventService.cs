using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceBase.Events;
using ServiceBase.IdentityServer.Config;
using ServiceBase.IdentityServer.Services;
using ServiceBase.Notification.Email;
using System;
using System.Threading.Tasks;

namespace ServiceBase.IdentityServer.Events
{
    public class DefaultEventService : IEventService
    {
        private readonly EventServiceHelper _helper;
        private readonly ILogger _logger;
        private readonly EventOptions _options;
        private readonly IEmailService _emailService;
        private readonly ApplicationOptions _applicationOptions;
        private readonly IUserAccountStore _userAccountStore;

        public DefaultEventService(
            ILogger<DefaultEventService> logger,
            IOptions<ApplicationOptions> applicationOptions,
            IOptions<EventOptions> options,
            IHttpContextAccessor context,
            IEmailService emailService,
            IUserAccountStore userAccountStore)
        {
            _logger = logger;
            _options = options.Value;
            _applicationOptions = applicationOptions.Value;
            _helper = new EventServiceHelper(_options, context);
            _emailService = emailService;
            _userAccountStore = userAccountStore;
        }

        /// <summary>
        /// Raises the specified event.
        /// </summary>
        /// <param name="evt">The event.</param>
        /// <exception cref="System.ArgumentNullException">evt</exception>
        public async virtual Task RaiseAsync<T>(Event<T> evt)
        {
            if (evt == null) throw new ArgumentNullException(nameof(evt));

            if (_helper.CanRaiseEvent(evt))
            {
                _logger.LogInformation(_helper.PrepareEvent(evt));
            }

            // Send emails and/or SMS
            if (evt.EventType == EventTypes.Success)
            {
                if (_applicationOptions.RequireLocalAccountVerification && evt.Id == EventConstants.Ids.UserAccountCreated)
                {
                    var details = evt.Details as UserAccountCreatedDetails;
                    if (details != null)
                    {
                        var userAccount = await _userAccountStore.LoadByIdAsync(details.UserAccountId);
                        if (userAccount != null)
                        {
                            await _emailService.SendEmailAsync("UserAccountCreated", userAccount.Email, new
                            {
                                // TODO: change to read x-forwared-host or use event context
                                ConfirmUrl = String.Format("http://localhost/register/confirm/{0}", userAccount.VerificationKey),
                                CancelUrl = String.Format("http://localhost/register/cancel/{0}", userAccount.VerificationKey)
                            });
                        }
                    }
                }
            }
        }
    }
}

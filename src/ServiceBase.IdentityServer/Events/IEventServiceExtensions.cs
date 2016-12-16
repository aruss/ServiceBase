using ServiceBase.Events;
using System;
using System.Threading.Tasks;

namespace ServiceBase.IdentityServer.Events
{
    public static class IEventServiceExtensions
    {
        public static async Task RaiseSuccessfulUserAccountCreatedEventAsync(this IEventService events, Guid userAccountId, string provider)
        {
            var evt = new Event<UserAccountCreatedDetails>(
                EventConstants.Categories.UserAccount,
                "User Account Creation Success",
                EventTypes.Success,
                EventConstants.Ids.UserAccountCreated,
                new UserAccountCreatedDetails
                {
                    UserAccountId = userAccountId,
                    Provider = provider
                });

            await events.RaiseAsync(evt);
        }
    }
}

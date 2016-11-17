using IdentityServer4.Events;
using IdentityServer4.Services;
using System.Threading.Tasks;

namespace ServiceBase.IdentityServer.Events
{
    internal static class IEventServiceExtensions
    {
        public static async Task RaiseSuccessfulUserRegisteredEventAsync(this IEventService events,
            string userName, string subjectId)
        {
            /*var evt = new Event<LocalLoginDetails>(
                EventConstants.Categories.Authentication,
                "Resource Owner Flow Login Success",
                EventTypes.Success,
                EventConstants.Ids.ResourceOwnerFlowLoginSuccess,
                new LocalLoginDetails
                {
                    SubjectId = subjectId,
                    LoginUserName = userName
                });

            await events.RaiseAsync(evt);*/
        }
    }
}

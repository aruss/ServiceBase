using ServiceBase.IdentityServer.Public.UI.Login;
using System.Collections.Generic;

namespace ServiceBase.IdentityServer.Public.UI.Recover
{
    public class RecoverViewModel : RecoverInputModel, IExternalLoginsViewModel
    {
        public RecoverViewModel()
        {
        }

        public RecoverViewModel(RecoverInputModel other)
        {
            this.Email = other.Email;
        }

        public string ErrorMessage { get; set; }

        public IEnumerable<ExternalProvider> ExternalProviders { get; set; }
    }
}
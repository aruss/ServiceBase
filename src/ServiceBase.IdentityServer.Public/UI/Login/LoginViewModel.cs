using System.Collections.Generic;

namespace ServiceBase.IdentityServer.Public.UI.Login
{
    public class LoginViewModel : LoginInputModel, IExternalLoginsViewModel
    {
        public LoginViewModel()
        {
        }

        public LoginViewModel(LoginInputModel other)
        {
            Email = other.Email;
            Password = other.Password;
            RememberLogin = other.RememberLogin;
            ReturnUrl = other.ReturnUrl;
        }

        public string ErrorMessage { get; set; }
        public string InfoMessage { get; set; }
        public bool EnableLocalLogin { get; set; }
        public IEnumerable<ExternalProvider> ExternalProviders { get; set; }
        public IEnumerable<string> LoginHints { get; set; }
    }

    public class ExternalProvider
    {
        public string DisplayName { get; set; }
        public string AuthenticationScheme { get; set; }
    }

    public interface IExternalLoginsViewModel
    {
        IEnumerable<ExternalProvider> ExternalProviders { get; set; }
        string ReturnUrl { get; set; }
    }
}
namespace ServiceBase.IdentityServer.UI.Login
{
    public class RegisterViewModel : RegisterInputModel
    {
        public string[] HintExternalAccounts { get; set; }

        public RegisterViewModel()
        {
        }

        public RegisterViewModel(RegisterInputModel other)
        {
            this.Email = other.Email;
            this.Password = other.Password;
            this.PasswordConfirm = other.PasswordConfirm;
        }

        public string ErrorMessage { get; set; }
    }
}
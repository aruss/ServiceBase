namespace ServiceBase.IdentityServer.Public.UI.Login
{
    public class RecoverViewModel : RecoverInputModel
    {
        public RecoverViewModel()
        {
        }

        public RecoverViewModel(RecoverInputModel other)
        {
            this.Email = other.Email;
        }

        public string ErrorMessage { get; set; }
    }
}
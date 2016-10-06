using System.ComponentModel.DataAnnotations;

namespace ServiceBase.IdentityServer.Public.UI.Login
{
    public class RecoverInputModel
    {
        [Required]
        public string Email { get; set; }

        public string ReturnUrl { get; set; }
    }
}
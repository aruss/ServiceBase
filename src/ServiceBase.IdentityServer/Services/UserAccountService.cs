using Host.Config;
using Host.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Host.Services
{
    public class UserAccountService
    {
        ApplicationOptions _options;
        IUserAccountStore _userAccountStore; 

        public UserAccountService(
            IOptions<ApplicationOptions> options,
            IUserAccountStore userAccountStore
        )
        {
            _options = options.Value;
            _userAccountStore = userAccountStore; 

        }

        public bool AccountAlreadyExists(string email)
        {
            return _userAccountStore.LoadByEmailAsync(email) != null; 
        }

        public UserAccount CreateAccount(string email, string password)
        {


            return null; 
        }
    }
}

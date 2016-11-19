using IdentityServer4.EntityFramework.Options;
using System;

namespace ServiceBase.IdentityServer.EntityFramework.Options
{
    public class DefaultStoreOptions
    {
        /*public DefaultStoreOptions() { }

        public DefaultStoreOptions(string tablePrefix)
        {
            if (!String.IsNullOrWhiteSpace(tablePrefix))
            {
                this.UserAccount.Schema = String.Format("{0}{1}", tablePrefix, this.UserAccount.Schema);
                this.ExternalAccount.Schema = String.Format("{0}{1}", tablePrefix, this.ExternalAccount.Schema);
                this.UserClaim.Schema = String.Format("{0}{1}", tablePrefix, this.UserClaim.Schema);
            }
        }*/

        public string DefaultSchema { get; set; } = null;

        public TableConfiguration UserAccount { get; set; } = new TableConfiguration("UserAccounts");
        public TableConfiguration ExternalAccount { get; set; } = new TableConfiguration("ExternalAccounts");
        public TableConfiguration UserClaim { get; set; } = new TableConfiguration("UserClaims");
    }
}

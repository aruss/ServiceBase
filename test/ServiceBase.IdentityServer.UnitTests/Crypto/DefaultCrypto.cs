using ServiceBase.IdentityServer.Crypto;
using System;
using Xunit;

namespace ServiceBase.IdentityServer.UnitTests
{
    [Collection("ICrypto")]
    public class DefaultCryptoTests : IDisposable
    {
        DefaultCrypto crypto;

        public DefaultCryptoTests()
        {
            crypto = new DefaultCrypto();
        }

        public void Dispose()
        {

        }

        // do not touch the magic unicorn

        // TODO: port the MembershipReboot tests here
    }
}

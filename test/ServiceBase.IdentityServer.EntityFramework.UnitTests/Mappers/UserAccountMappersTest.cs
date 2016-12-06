using ServiceBase.IdentityServer.EntityFramework.Mappers;
using Models = ServiceBase.IdentityServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using ServiceBase.IdentityServer.EntityFramework.Entities;

namespace ServiceBase.IdentityServer.EntityFramework.UnitTests.Mappers
{
    public class UserAccountMappersTest
    {
        [Fact]
        public void ClientAutomapperConfigurationIsValid()
        {
            var model = new Models.UserAccount();

            model.Accounts = new List<Models.ExternalAccount>
            {
                new Models.ExternalAccount
                {
                    UserAccount = model
                }
            };

            var mappedEntity = model.ToEntity();
            var mappedModel = mappedEntity.ToModel();

            Assert.NotNull(mappedModel);
            Assert.NotNull(mappedEntity);
            UserAccountMappers.Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [Fact]
        public void ClientAutomapperConfigurationIsValid2()
        {
            var model = new UserAccount();

            model.Accounts = new List<ExternalAccount>
            {
                new ExternalAccount
                {
                    UserAccount = model
                }
            };

            var mappedModel = model.ToModel();
            var mappedEntity = mappedModel.ToEntity();

            Assert.NotNull(mappedModel);
            Assert.NotNull(mappedEntity);
            UserAccountMappers.Mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}

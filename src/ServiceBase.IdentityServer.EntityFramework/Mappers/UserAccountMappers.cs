using AutoMapper;
using ServiceBase.IdentityServer.EntityFramework.Entities;

namespace ServiceBase.IdentityServer.EntityFramework.Mappers
{
    public static class UserAccountMappers
    {
        static UserAccountMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<UserAccountProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        public static Models.UserAccount ToModel(this UserAccount userAccount)
        {
            return userAccount == null ? null : Mapper.Map<Models.UserAccount>(userAccount);
        }

        public static UserAccount ToEntity(this Models.UserAccount userAccount)
        {
            return userAccount == null ? null : Mapper.Map<UserAccount>(userAccount);
        }

        public static void UpdateEntity(this Models.UserAccount token, UserAccount target)
        {
            Mapper.Map(token, target);
        }

        public static Models.ExternalAccount ToModel(this ExternalAccount userAccount)
        {
            return userAccount == null ? null : Mapper.Map<Models.ExternalAccount>(userAccount);
        }

        public static ExternalAccount ToEntity(this Models.ExternalAccount userAccount)
        {
            return userAccount == null ? null : Mapper.Map<ExternalAccount>(userAccount);
        }

        public static void UpdateEntity(this Models.ExternalAccount token, ExternalAccount target)
        {
            Mapper.Map(token, target);
        }
    }
}
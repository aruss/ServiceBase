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
    }
}

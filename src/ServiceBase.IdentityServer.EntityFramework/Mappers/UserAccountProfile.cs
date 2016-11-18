using AutoMapper;
using ServiceBase.IdentityServer.EntityFramework.Entities;

namespace ServiceBase.IdentityServer.EntityFramework.Mappers
{
    public class UserAccountProfile : Profile
    {
        public UserAccountProfile()
        {
            CreateMap<UserAccount, Models.UserAccount>(MemberList.Destination);

            CreateMap<ExternalAccount, Models.ExternalAccount>(MemberList.Destination);

            CreateMap<UserClaim, Models.UserClaim>(MemberList.Destination);
        }
    }
}

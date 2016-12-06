using AutoMapper;
using ServiceBase.IdentityServer.EntityFramework.Entities;
using System.Linq;
using System.Security.Claims;
using IdentityServer4.EntityFramework.Entities;


namespace ServiceBase.IdentityServer.EntityFramework.Mappers
{
    public class UserAccountProfile : Profile
    {
        public UserAccountProfile()
        {
            CreateMap<UserAccount, Models.UserAccount>(MemberList.Destination)
                .PreserveReferences();
            CreateMap<Models.UserAccount, UserAccount>(MemberList.Source)
                .PreserveReferences();

            CreateMap<ExternalAccount, Models.ExternalAccount>(MemberList.Destination)
                .PreserveReferences();
            CreateMap<Models.ExternalAccount, ExternalAccount>(MemberList.Source)
                .PreserveReferences();

            CreateMap<UserClaim, Models.UserClaim>(MemberList.Destination)
                .PreserveReferences();
            CreateMap<Models.UserClaim, UserClaim>(MemberList.Source)
                .PreserveReferences();
        }
    }
}

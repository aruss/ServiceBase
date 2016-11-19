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
            CreateMap<UserAccount, Models.UserAccount>(MemberList.Destination); 
            CreateMap<Models.UserAccount, UserAccount>(MemberList.Source);

            CreateMap<ExternalAccount, Models.ExternalAccount>(MemberList.Destination);
            CreateMap<Models.ExternalAccount, ExternalAccount>(MemberList.Source);

            CreateMap<UserClaim, Models.UserClaim>(MemberList.Destination);
            CreateMap<Models.UserClaim, UserClaim>(MemberList.Source);
        }
    }
}

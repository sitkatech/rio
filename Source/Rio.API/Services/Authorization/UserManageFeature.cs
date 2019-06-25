using Rio.EFModels.Entities;

namespace Rio.API.Services.Authorization
{
    public class UserManageFeature : BaseAuthorizationAttribute
    {
        public UserManageFeature() : base(new []{RoleEnum.Admin, RoleEnum.SitkaAdmin})
        {
        }
    }
}
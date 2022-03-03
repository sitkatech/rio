using Rio.EFModels.Entities;

namespace Rio.API.Services.Authorization
{
    public class UserViewFeature : BaseAuthorizationAttribute
    {
        public UserViewFeature() : base(new []{RoleEnum.Admin, RoleEnum.LandOwner, RoleEnum.DemoUser, RoleEnum.Unassigned})
        {
        }
    }
}
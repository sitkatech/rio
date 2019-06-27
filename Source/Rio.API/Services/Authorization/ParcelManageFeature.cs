using Rio.EFModels.Entities;

namespace Rio.API.Services.Authorization
{
    public class ParcelManageFeature : BaseAuthorizationAttribute
    {
        public ParcelManageFeature() : base(new []{RoleEnum.Admin})
        {
        }
    }
}
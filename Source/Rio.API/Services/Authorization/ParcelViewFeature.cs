using Rio.EFModels.Entities;

namespace Rio.API.Services.Authorization
{
    public class ParcelViewFeature : BaseAuthorizationAttribute
    {
        public ParcelViewFeature() : base(new []{RoleEnum.Admin, RoleEnum.LandOwner})
        {
        }
    }
}
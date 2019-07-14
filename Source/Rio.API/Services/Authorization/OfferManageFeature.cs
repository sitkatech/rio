using Rio.EFModels.Entities;

namespace Rio.API.Services.Authorization
{
    public class OfferManageFeature : BaseAuthorizationAttribute
    {
        public OfferManageFeature() : base(new []{RoleEnum.Admin, RoleEnum.LandOwner})
        {
        }
    }
}
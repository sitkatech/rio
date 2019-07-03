using Rio.EFModels.Entities;

namespace Rio.API.Services.Authorization
{
    public class PostingManageFeature : BaseAuthorizationAttribute
    {
        public PostingManageFeature() : base(new []{RoleEnum.Admin, RoleEnum.LandOwner})
        {
        }
    }
}
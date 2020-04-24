using Rio.EFModels.Entities;

namespace Rio.API.Services.Authorization
{
    public class ContentManageFeature : BaseAuthorizationAttribute
    {
        public ContentManageFeature() : base(new []{RoleEnum.Admin})
        {
        }
    }
}
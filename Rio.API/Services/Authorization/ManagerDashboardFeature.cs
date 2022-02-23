using Rio.EFModels.Entities;

namespace Rio.API.Services.Authorization
{
    public class ManagerDashboardFeature : BaseAuthorizationAttribute
    {
        public ManagerDashboardFeature() : base(new []{RoleEnum.Admin, RoleEnum.DemoUser})
        {
        }
    }
}
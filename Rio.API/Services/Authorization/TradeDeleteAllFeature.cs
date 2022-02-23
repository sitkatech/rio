using Rio.EFModels.Entities;

namespace Rio.API.Services.Authorization
{
    public class TradeDeleteAllFeature : BaseAuthorizationAttribute
    {
        public TradeDeleteAllFeature() : base(new[] { RoleEnum.Admin })
        {
        }
    }
}
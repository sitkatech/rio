using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects.WaterTransfer;

namespace Rio.EFModels.Entities
{
    public partial class WaterTransferRegistration
    {
        private static IQueryable<WaterTransferRegistration> GetWaterTransferRegistrationsImpl(RioDbContext dbContext)
        {
            return dbContext.WaterTransferRegistration
                .Include(x => x.User)
                .AsNoTracking();
        }

        public static IEnumerable<WaterTransferRegistrationSimpleDto> GetByWaterTransferID(RioDbContext dbContext, int waterTransferID)
        {
            var waterTransferRegistrations = GetWaterTransferRegistrationsImpl(dbContext).Where(x => x.WaterTransferID == waterTransferID);
            return waterTransferRegistrations.Select(x => x.AsSimpleDto()).AsEnumerable();
        }

        public bool IsPending =>
            WaterTransferRegistrationStatusID == (int) WaterTransferRegistrationStatusEnum.Pending;

        public bool IsRegistered =>
            WaterTransferRegistrationStatusID == (int) WaterTransferRegistrationStatusEnum.Registered;

        public bool IsCanceled =>
            WaterTransferRegistrationStatusID == (int) WaterTransferRegistrationStatusEnum.Canceled;
    }
}
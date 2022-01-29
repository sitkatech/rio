using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public partial class WaterTransferRegistration
    {
        private static IQueryable<WaterTransferRegistration> GetWaterTransferRegistrationsImpl(RioDbContext dbContext)
        {
            return dbContext.WaterTransferRegistrations
                .Include(x => x.Account)
                .AsNoTracking();
        }

        public static List<WaterTransferRegistrationSimpleDto> GetByWaterTransferID(RioDbContext dbContext, int waterTransferID)
        {
            var waterTransferRegistrations = GetWaterTransferRegistrationsImpl(dbContext).Where(x => x.WaterTransferID == waterTransferID);
            return waterTransferRegistrations.Select(x => x.AsSimpleDto()).ToList();
        }

        public bool IsPending =>
            WaterTransferRegistrationStatusID == (int) WaterTransferRegistrationStatusEnum.Pending;

        public bool IsRegistered =>
            WaterTransferRegistrationStatusID == (int) WaterTransferRegistrationStatusEnum.Registered;

        public bool IsCanceled =>
            WaterTransferRegistrationStatusID == (int) WaterTransferRegistrationStatusEnum.Canceled;

        public static void DeleteAll(RioDbContext dbContext)
        {
            dbContext.WaterTransferRegistrations.RemoveRange(dbContext.WaterTransferRegistrations);
            dbContext.SaveChanges();
        }
    }
}
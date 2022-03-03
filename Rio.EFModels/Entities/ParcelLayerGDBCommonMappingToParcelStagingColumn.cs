using Rio.Models.DataTransferObjects;
using System.Linq;

namespace Rio.EFModels.Entities
{
    public partial class ParcelLayerGDBCommonMappingToParcelStagingColumn
    {
        public static ParcelLayerGDBCommonMappingToParcelStagingColumnDto GetCommonMappings(RioDbContext _dbContext)
        {
            return _dbContext.ParcelLayerGDBCommonMappingToParcelStagingColumns.First().AsDto();
        }
    }
}
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static class WaterYearExtensionMethods
    {
        public static WaterYearDto AsDto(this WaterYear waterYear)
        {
            return new WaterYearDto()
            {
                WaterYearID = waterYear.WaterYearID,
                Year = waterYear.Year,
                ParcelLayerUpdateDate = waterYear.ParcelLayerUpdateDate
            };
        }
    }
}
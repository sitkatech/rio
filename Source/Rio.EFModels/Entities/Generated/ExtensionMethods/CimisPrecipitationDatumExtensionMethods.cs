//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[CimisPrecipitationDatum]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class CimisPrecipitationDatumExtensionMethods
    {
        public static CimisPrecipitationDatumDto AsDto(this CimisPrecipitationDatum cimisPrecipitationDatum)
        {
            var cimisPrecipitationDatumDto = new CimisPrecipitationDatumDto()
            {
                CimisPrecipitationDatumID = cimisPrecipitationDatum.CimisPrecipitationDatumID,
                DateMeasured = cimisPrecipitationDatum.DateMeasured,
                Precipitation = cimisPrecipitationDatum.Precipitation,
                LastUpdated = cimisPrecipitationDatum.LastUpdated
            };
            DoCustomMappings(cimisPrecipitationDatum, cimisPrecipitationDatumDto);
            return cimisPrecipitationDatumDto;
        }

        static partial void DoCustomMappings(CimisPrecipitationDatum cimisPrecipitationDatum, CimisPrecipitationDatumDto cimisPrecipitationDatumDto);

        public static CimisPrecipitationDatumSimpleDto AsSimpleDto(this CimisPrecipitationDatum cimisPrecipitationDatum)
        {
            var cimisPrecipitationDatumSimpleDto = new CimisPrecipitationDatumSimpleDto()
            {
                CimisPrecipitationDatumID = cimisPrecipitationDatum.CimisPrecipitationDatumID,
                DateMeasured = cimisPrecipitationDatum.DateMeasured,
                Precipitation = cimisPrecipitationDatum.Precipitation,
                LastUpdated = cimisPrecipitationDatum.LastUpdated
            };
            DoCustomSimpleDtoMappings(cimisPrecipitationDatum, cimisPrecipitationDatumSimpleDto);
            return cimisPrecipitationDatumSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(CimisPrecipitationDatum cimisPrecipitationDatum, CimisPrecipitationDatumSimpleDto cimisPrecipitationDatumSimpleDto);
    }
}
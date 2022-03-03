//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Well]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class WellExtensionMethods
    {
        public static WellDto AsDto(this Well well)
        {
            var wellDto = new WellDto()
            {
                WellID = well.WellID,
                WellName = well.WellName,
                WellType = well.WellType,
                WellTypeCode = well.WellTypeCode,
                WellTypeCodeName = well.WellTypeCodeName
            };
            DoCustomMappings(well, wellDto);
            return wellDto;
        }

        static partial void DoCustomMappings(Well well, WellDto wellDto);

        public static WellSimpleDto AsSimpleDto(this Well well)
        {
            var wellSimpleDto = new WellSimpleDto()
            {
                WellID = well.WellID,
                WellName = well.WellName,
                WellType = well.WellType,
                WellTypeCode = well.WellTypeCode,
                WellTypeCodeName = well.WellTypeCodeName
            };
            DoCustomSimpleDtoMappings(well, wellSimpleDto);
            return wellSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(Well well, WellSimpleDto wellSimpleDto);
    }
}
//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[CimisPrecipitationDatum]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class CimisPrecipitationDatumDto
    {
        public int CimisPrecipitationDatumID { get; set; }
        public DateTime DateMeasured { get; set; }
        public decimal Precipitation { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public partial class CimisPrecipitationDatumSimpleDto
    {
        public int CimisPrecipitationDatumID { get; set; }
        public DateTime DateMeasured { get; set; }
        public decimal Precipitation { get; set; }
        public DateTime LastUpdated { get; set; }
    }

}
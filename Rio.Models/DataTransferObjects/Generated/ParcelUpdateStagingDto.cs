//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelUpdateStaging]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class ParcelUpdateStagingDto
    {
        public int ParcelUpdateStagingID { get; set; }
        public string ParcelNumber { get; set; }
        public string OwnerName { get; set; }
        public string ParcelGeometryText { get; set; }
        public string ParcelGeometry4326Text { get; set; }
        public bool HasConflict { get; set; }
    }

    public partial class ParcelUpdateStagingSimpleDto
    {
        public int ParcelUpdateStagingID { get; set; }
        public string ParcelNumber { get; set; }
        public string OwnerName { get; set; }
        public string ParcelGeometryText { get; set; }
        public string ParcelGeometry4326Text { get; set; }
        public bool HasConflict { get; set; }
    }

}
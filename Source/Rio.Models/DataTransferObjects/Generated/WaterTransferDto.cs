//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterTransfer]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class WaterTransferDto
    {
        public int WaterTransferID { get; set; }
        public DateTime TransferDate { get; set; }
        public int AcreFeetTransferred { get; set; }
        public OfferDto Offer { get; set; }
        public string Notes { get; set; }
    }

    public partial class WaterTransferSimpleDto
    {
        public int WaterTransferID { get; set; }
        public DateTime TransferDate { get; set; }
        public int AcreFeetTransferred { get; set; }
        public int OfferID { get; set; }
        public string Notes { get; set; }
    }

}
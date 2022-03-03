//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[DisadvantagedCommunity]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class DisadvantagedCommunityDto
    {
        public int DisadvantagedCommunityID { get; set; }
        public string DisadvantagedCommunityName { get; set; }
        public int LSADCode { get; set; }
        public DisadvantagedCommunityStatusDto DisadvantagedCommunityStatus { get; set; }
    }

    public partial class DisadvantagedCommunitySimpleDto
    {
        public int DisadvantagedCommunityID { get; set; }
        public string DisadvantagedCommunityName { get; set; }
        public int LSADCode { get; set; }
        public int DisadvantagedCommunityStatusID { get; set; }
    }

}
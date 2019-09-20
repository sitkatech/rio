using System;

namespace Rio.EFModels.Entities
{
    public partial class PostingDetailed
    {
        public PostingDetailed()
        {
        }

        public int PostingID { get; set; }
        public DateTime PostingDate { get; set; }
        public int PostingTypeID { get; set; }
        public string PostingTypeDisplayName { get; set; }

        public int PostingStatusID { get; set; }
        public string PostingStatusDisplayName { get; set; }

        public int PostedByUserID { get; set; }
        public string PostedByFirstName { get; set; }
        public string PostedByLastName { get; set; }
        public string PostedByEmail { get; set; }

        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int AvailableQuantity { get; set; }
        public int NumberOfOffers { get; set; }
    }
}
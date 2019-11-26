namespace Rio.Models.DataTransferObjects.Parcel
{
    public class ParcelOwnershipDto
    {
        public string OwnerName { get; set; }
        public int? OwnerUserID { get; set; }
        public int? EffectiveYear { get; set; }
        public string SaleDate { get; set; }

        public string Note { get; set; }
    }
}

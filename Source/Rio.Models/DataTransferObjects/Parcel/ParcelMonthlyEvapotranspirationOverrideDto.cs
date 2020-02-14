namespace Rio.Models.DataTransferObjects.Parcel
{
    public class ParcelMonthlyEvapotranspirationOverrideDto
    {
        public int ParcelID { get; set; }
        public string ParcelNumber { get; set; }
        public int WaterYear { get; set; }
        public int WaterMonth { get; set; }
        public decimal OverriddenEvapotranspirationRate { get; set; }
    }
}

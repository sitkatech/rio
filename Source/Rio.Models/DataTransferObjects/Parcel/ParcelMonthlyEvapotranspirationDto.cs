namespace Rio.Models.DataTransferObjects.Parcel
{
    public class ParcelMonthlyEvapotranspirationDto
    {
        public int WaterYear { get; set; }
        public int WaterMonth { get; set; }
        public decimal EvapotranspirationRate { get; set; }
    }
}
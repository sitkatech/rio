namespace Rio.Models.DataTransferObjects.Parcel
{
    public class ParcelDto
    {
        public int ParcelID { get; set; }
        public string ParcelNumber { get; set; }
        public static string GetListRoute => "parcels";

        public static string GetSingleRoute(int parcelID)
        {
            return $"parcels/{parcelID}";
        }
    }
}
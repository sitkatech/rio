namespace Rio.Models.DataTransferObjects
{
    public partial class WaterTransferRegistrationDto
    {
        public bool IsRegistered { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsPending { get; set; }
    }
}
namespace Rio.Models.DataTransferObjects
{
    public partial class ParcelLedgerDto
    {
        public int WaterYear => EffectiveDate.Year;
        public int WaterMonth => EffectiveDate.Month;
        public decimal TransactionDepth => TransactionAmount / Parcel.ParcelAreaInSquareFeet;
    }
}
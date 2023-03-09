namespace Rio.Models.DataTransferObjects;

public partial class ParcelUsageStagingSimpleDto
{
    public decimal ExistingAnnualUsageAmount { get; set; }
    public decimal ExistingMonthlyUsageAmount { get; set; }
    public decimal UpdatedAnnualUsageAmount { get; set; }
    public decimal UpdatedMonthlyUsageAmount { get; set; }
}
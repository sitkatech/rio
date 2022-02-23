using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities
{
    public partial class RioDbContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LandownerUsageReport>().HasNoKey();
            modelBuilder.Entity<ParcelWaterSupplyAndUsage>().HasNoKey();
        }
        public virtual DbSet<LandownerUsageReport> LandownerUsageReports { get; set; }
        public virtual DbSet<ParcelWaterSupplyAndUsage> ParcelWaterSupplyAndUsages { get; set; }
    }
}
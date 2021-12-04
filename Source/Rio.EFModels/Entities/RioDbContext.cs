using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities
{
    public partial class RioDbContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LandownerUsageReport>().HasNoKey();
            modelBuilder.Entity<ParcelAllocationAndUsage>().HasNoKey();
            modelBuilder.Entity<ParcelLedgerBulkCreateParcelReport>().HasNoKey();
        }
        public virtual DbSet<LandownerUsageReport> LandownerUsageReports { get; set; }
        public virtual DbSet<ParcelAllocationAndUsage> ParcelAllocationAndUsages { get; set; }
        public virtual DbSet<ParcelLedgerBulkCreateParcelReport> ParcelLedgerBulkCreateParcelReport { get; set; }
    }
}
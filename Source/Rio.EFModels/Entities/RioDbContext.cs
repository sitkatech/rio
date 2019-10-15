using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities
{
    public partial class RioDbContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LandownerUsageReport>().HasNoKey();
        }
    }
}
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Rio.EFModels.Entities
{
    public partial class RioDbContext : DbContext
    {
        public RioDbContext()
        {
        }

        public RioDbContext(DbContextOptions<RioDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DatabaseMigration> DatabaseMigration { get; set; }
        public virtual DbSet<FileResource> FileResource { get; set; }
        public virtual DbSet<FileResourceMimeType> FileResourceMimeType { get; set; }
        public virtual DbSet<Offer> Offer { get; set; }
        public virtual DbSet<OfferStatus> OfferStatus { get; set; }
        public virtual DbSet<Parcel> Parcel { get; set; }
        public virtual DbSet<ParcelAllocation> ParcelAllocation { get; set; }
        public virtual DbSet<ParcelAllocationType> ParcelAllocationType { get; set; }
        public virtual DbSet<ParcelMonthlyEvapotranspiration> ParcelMonthlyEvapotranspiration { get; set; }
        public virtual DbSet<Posting> Posting { get; set; }
        public virtual DbSet<PostingStatus> PostingStatus { get; set; }
        public virtual DbSet<PostingType> PostingType { get; set; }
        public virtual DbSet<RioPage> RioPage { get; set; }
        public virtual DbSet<RioPageImage> RioPageImage { get; set; }
        public virtual DbSet<RioPageType> RioPageType { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Trade> Trade { get; set; }
        public virtual DbSet<TradeStatus> TradeStatus { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserParcel> UserParcel { get; set; }
        public virtual DbSet<WaterTransfer> WaterTransfer { get; set; }
        public virtual DbSet<WaterTransferRegistration> WaterTransferRegistration { get; set; }
        public virtual DbSet<WaterTransferRegistrationParcel> WaterTransferRegistrationParcel { get; set; }
        public virtual DbSet<WaterTransferRegistrationStatus> WaterTransferRegistrationStatus { get; set; }
        public virtual DbSet<WaterTransferType> WaterTransferType { get; set; }
        public virtual DbSet<vAllParcelsWithAnnualWaterUsage> vAllParcelsWithAnnualWaterUsage { get; set; }
        public virtual DbSet<vGeoServerAllParcels> vGeoServerAllParcels { get; set; }
        public virtual DbSet<vParcelOwnership> vParcelOwnership { get; set; }
        public virtual DbSet<vPostingDetailed> vPostingDetailed { get; set; }
        public virtual DbSet<vUserDetailed> vUserDetailed { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DatabaseMigration>(entity =>
            {
                entity.HasKey(e => e.DatabaseMigrationNumber)
                    .HasName("PK_DatabaseMigration_DatabaseMigrationNumber");

                entity.HasIndex(e => e.ReleaseScriptFileName)
                    .HasName("UC_DatabaseMigration_ReleaseScriptFileName")
                    .IsUnique();

                entity.Property(e => e.DateMigrated).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.MigrationAuthorName).IsUnicode(false);

                entity.Property(e => e.MigrationReason).IsUnicode(false);

                entity.Property(e => e.ReleaseScriptFileName).IsUnicode(false);
            });

            modelBuilder.Entity<FileResource>(entity =>
            {
                entity.HasIndex(e => e.FileResourceGUID)
                    .HasName("AK_FileResource_FileResourceGUID")
                    .IsUnique();

                entity.Property(e => e.OriginalBaseFilename).IsUnicode(false);

                entity.Property(e => e.OriginalFileExtension).IsUnicode(false);

                entity.HasOne(d => d.CreateUser)
                    .WithMany(p => p.FileResource)
                    .HasForeignKey(d => d.CreateUserID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FileResource_User_CreateUserID_UserID");

                entity.HasOne(d => d.FileResourceMimeType)
                    .WithMany(p => p.FileResource)
                    .HasForeignKey(d => d.FileResourceMimeTypeID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<FileResourceMimeType>(entity =>
            {
                entity.HasIndex(e => e.FileResourceMimeTypeDisplayName)
                    .HasName("AK_FileResourceMimeType_FileResourceMimeTypeDisplayName")
                    .IsUnique();

                entity.HasIndex(e => e.FileResourceMimeTypeName)
                    .HasName("AK_FileResourceMimeType_FileResourceMimeTypeName")
                    .IsUnique();

                entity.Property(e => e.FileResourceMimeTypeID).ValueGeneratedNever();

                entity.Property(e => e.FileResourceMimeTypeContentTypeName).IsUnicode(false);

                entity.Property(e => e.FileResourceMimeTypeDisplayName).IsUnicode(false);

                entity.Property(e => e.FileResourceMimeTypeIconNormalFilename).IsUnicode(false);

                entity.Property(e => e.FileResourceMimeTypeIconSmallFilename).IsUnicode(false);

                entity.Property(e => e.FileResourceMimeTypeName).IsUnicode(false);
            });

            modelBuilder.Entity<Offer>(entity =>
            {
                entity.Property(e => e.OfferNotes).IsUnicode(false);

                entity.HasOne(d => d.CreateUser)
                    .WithMany(p => p.Offer)
                    .HasForeignKey(d => d.CreateUserID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Offer_User_CreateUserID_UserID");

                entity.HasOne(d => d.OfferStatus)
                    .WithMany(p => p.Offer)
                    .HasForeignKey(d => d.OfferStatusID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Trade)
                    .WithMany(p => p.Offer)
                    .HasForeignKey(d => d.TradeID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<OfferStatus>(entity =>
            {
                entity.HasIndex(e => e.OfferStatusDisplayName)
                    .HasName("AK_OfferStatus_OfferStatusDisplayName")
                    .IsUnique();

                entity.HasIndex(e => e.OfferStatusName)
                    .HasName("AK_OfferStatus_OfferStatusName")
                    .IsUnique();

                entity.Property(e => e.OfferStatusID).ValueGeneratedNever();

                entity.Property(e => e.OfferStatusDisplayName).IsUnicode(false);

                entity.Property(e => e.OfferStatusName).IsUnicode(false);
            });

            modelBuilder.Entity<Parcel>(entity =>
            {
                entity.HasIndex(e => e.ParcelNumber)
                    .HasName("AK_Parcel_ParcelNumber")
                    .IsUnique();

                entity.Property(e => e.OwnerAddress).IsUnicode(false);

                entity.Property(e => e.OwnerCity).IsUnicode(false);

                entity.Property(e => e.OwnerName).IsUnicode(false);

                entity.Property(e => e.OwnerZipCode).IsUnicode(false);

                entity.Property(e => e.ParcelNumber).IsUnicode(false);
            });

            modelBuilder.Entity<ParcelAllocation>(entity =>
            {
                entity.HasIndex(e => new { e.ParcelID, e.WaterYear, e.ParcelAllocationTypeID })
                    .HasName("AK_ParcelAllocation_ParcelID_WaterYear")
                    .IsUnique();

                entity.HasOne(d => d.ParcelAllocationType)
                    .WithMany(p => p.ParcelAllocation)
                    .HasForeignKey(d => d.ParcelAllocationTypeID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Parcel)
                    .WithMany(p => p.ParcelAllocation)
                    .HasForeignKey(d => d.ParcelID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ParcelAllocationType>(entity =>
            {
                entity.HasIndex(e => e.ParcelAllocationTypeDisplayName)
                    .HasName("AK_ParcelAllocationType_ParcelAllocationTypeDisplayName")
                    .IsUnique();

                entity.HasIndex(e => e.ParcelAllocationTypeName)
                    .HasName("AK_ParcelAllocationType_ParcelAllocationTypeName")
                    .IsUnique();

                entity.Property(e => e.ParcelAllocationTypeID).ValueGeneratedNever();

                entity.Property(e => e.ParcelAllocationTypeDisplayName).IsUnicode(false);

                entity.Property(e => e.ParcelAllocationTypeName).IsUnicode(false);
            });

            modelBuilder.Entity<ParcelMonthlyEvapotranspiration>(entity =>
            {
                entity.HasIndex(e => new { e.ParcelID, e.WaterYear, e.WaterMonth })
                    .HasName("AK_ParcelMonthlyEvapotranspiration_ParcelID_WaterYear_WaterMonth")
                    .IsUnique();

                entity.HasOne(d => d.Parcel)
                    .WithMany(p => p.ParcelMonthlyEvapotranspiration)
                    .HasForeignKey(d => d.ParcelID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Posting>(entity =>
            {
                entity.Property(e => e.PostingDescription).IsUnicode(false);

                entity.HasOne(d => d.CreateUser)
                    .WithMany(p => p.Posting)
                    .HasForeignKey(d => d.CreateUserID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Posting_User_CreateUserID_UserID");

                entity.HasOne(d => d.PostingStatus)
                    .WithMany(p => p.Posting)
                    .HasForeignKey(d => d.PostingStatusID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.PostingType)
                    .WithMany(p => p.Posting)
                    .HasForeignKey(d => d.PostingTypeID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PostingStatus>(entity =>
            {
                entity.HasIndex(e => e.PostingStatusDisplayName)
                    .HasName("AK_PostingStatus_PostingStatusDisplayName")
                    .IsUnique();

                entity.HasIndex(e => e.PostingStatusName)
                    .HasName("AK_PostingStatus_PostingStatusName")
                    .IsUnique();

                entity.Property(e => e.PostingStatusID).ValueGeneratedNever();

                entity.Property(e => e.PostingStatusDisplayName).IsUnicode(false);

                entity.Property(e => e.PostingStatusName).IsUnicode(false);
            });

            modelBuilder.Entity<PostingType>(entity =>
            {
                entity.HasIndex(e => e.PostingTypeDisplayName)
                    .HasName("AK_PostingType_PostingTypeDisplayName")
                    .IsUnique();

                entity.HasIndex(e => e.PostingTypeName)
                    .HasName("AK_PostingType_PostingTypeName")
                    .IsUnique();

                entity.Property(e => e.PostingTypeID).ValueGeneratedNever();

                entity.Property(e => e.PostingTypeDisplayName).IsUnicode(false);

                entity.Property(e => e.PostingTypeName).IsUnicode(false);
            });

            modelBuilder.Entity<RioPage>(entity =>
            {
                entity.HasIndex(e => e.RioPageTypeID)
                    .HasName("AK_RioPage_RioPageTypeID")
                    .IsUnique();

                entity.Property(e => e.RioPageContent).IsUnicode(false);
            });

            modelBuilder.Entity<RioPageImage>(entity =>
            {
                entity.HasIndex(e => new { e.RioPageImageID, e.FileResourceID })
                    .HasName("AK_RioPageImage_RioPageImageID_FileResourceID")
                    .IsUnique();

                entity.HasOne(d => d.FileResource)
                    .WithMany(p => p.RioPageImage)
                    .HasForeignKey(d => d.FileResourceID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.RioPage)
                    .WithMany(p => p.RioPageImage)
                    .HasForeignKey(d => d.RioPageID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<RioPageType>(entity =>
            {
                entity.HasIndex(e => e.RioPageTypeDisplayName)
                    .HasName("AK_RioPageType_RioPageTypeDisplayName")
                    .IsUnique();

                entity.HasIndex(e => e.RioPageTypeName)
                    .HasName("AK_RioPageType_RioPageTypeName")
                    .IsUnique();

                entity.Property(e => e.RioPageTypeID).ValueGeneratedNever();

                entity.Property(e => e.RioPageTypeDisplayName).IsUnicode(false);

                entity.Property(e => e.RioPageTypeName).IsUnicode(false);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasIndex(e => e.RoleDisplayName)
                    .HasName("AK_Role_RoleDisplayName")
                    .IsUnique();

                entity.HasIndex(e => e.RoleName)
                    .HasName("AK_Role_RoleName")
                    .IsUnique();

                entity.Property(e => e.RoleID).ValueGeneratedNever();

                entity.Property(e => e.RoleDescription).IsUnicode(false);

                entity.Property(e => e.RoleDisplayName).IsUnicode(false);

                entity.Property(e => e.RoleName).IsUnicode(false);
            });

            modelBuilder.Entity<Trade>(entity =>
            {
                entity.Property(e => e.TradeNumber).IsUnicode(false);

                entity.HasOne(d => d.CreateUser)
                    .WithMany(p => p.Trade)
                    .HasForeignKey(d => d.CreateUserID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Trade_User_CreateUserID_UserID");

                entity.HasOne(d => d.Posting)
                    .WithMany(p => p.Trade)
                    .HasForeignKey(d => d.PostingID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.TradeStatus)
                    .WithMany(p => p.Trade)
                    .HasForeignKey(d => d.TradeStatusID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<TradeStatus>(entity =>
            {
                entity.HasIndex(e => e.TradeStatusDisplayName)
                    .HasName("AK_TradeStatus_TradeStatusDisplayName")
                    .IsUnique();

                entity.HasIndex(e => e.TradeStatusName)
                    .HasName("AK_TradeStatus_TradeStatusName")
                    .IsUnique();

                entity.Property(e => e.TradeStatusID).ValueGeneratedNever();

                entity.Property(e => e.TradeStatusDisplayName).IsUnicode(false);

                entity.Property(e => e.TradeStatusName).IsUnicode(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email)
                    .HasName("AK_User_Email")
                    .IsUnique();

                entity.HasIndex(e => e.UserGuid)
                    .HasName("AK_User_UserGuid")
                    .IsUnique()
                    .HasFilter("([UserGuid] IS NOT NULL)");

                entity.Property(e => e.Company).IsUnicode(false);

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.FirstName).IsUnicode(false);

                entity.Property(e => e.LastName).IsUnicode(false);

                entity.Property(e => e.LoginName).IsUnicode(false);

                entity.Property(e => e.Phone).IsUnicode(false);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.RoleID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<UserParcel>(entity =>
            {
                entity.Property(e => e.Note).IsUnicode(false);

                entity.Property(e => e.OwnerName).IsUnicode(false);

                entity.HasOne(d => d.Parcel)
                    .WithMany(p => p.UserParcel)
                    .HasForeignKey(d => d.ParcelID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<WaterTransfer>(entity =>
            {
                entity.Property(e => e.Notes).IsUnicode(false);

                entity.HasOne(d => d.Offer)
                    .WithMany(p => p.WaterTransfer)
                    .HasForeignKey(d => d.OfferID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<WaterTransferRegistration>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.WaterTransferRegistration)
                    .HasForeignKey(d => d.UserID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.WaterTransfer)
                    .WithMany(p => p.WaterTransferRegistration)
                    .HasForeignKey(d => d.WaterTransferID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.WaterTransferRegistrationStatus)
                    .WithMany(p => p.WaterTransferRegistration)
                    .HasForeignKey(d => d.WaterTransferRegistrationStatusID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.WaterTransferType)
                    .WithMany(p => p.WaterTransferRegistration)
                    .HasForeignKey(d => d.WaterTransferTypeID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<WaterTransferRegistrationParcel>(entity =>
            {
                entity.HasOne(d => d.Parcel)
                    .WithMany(p => p.WaterTransferRegistrationParcel)
                    .HasForeignKey(d => d.ParcelID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.WaterTransferRegistration)
                    .WithMany(p => p.WaterTransferRegistrationParcel)
                    .HasForeignKey(d => d.WaterTransferRegistrationID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<WaterTransferRegistrationStatus>(entity =>
            {
                entity.HasIndex(e => e.WaterTransferRegistrationStatusDisplayName)
                    .HasName("AK_WaterTransferRegistrationStatus_WaterTransferRegistrationStatusDisplayName")
                    .IsUnique();

                entity.HasIndex(e => e.WaterTransferRegistrationStatusName)
                    .HasName("AK_WaterTransferRegistrationStatus_WaterTransferRegistrationStatusName")
                    .IsUnique();

                entity.Property(e => e.WaterTransferRegistrationStatusID).ValueGeneratedNever();

                entity.Property(e => e.WaterTransferRegistrationStatusDisplayName).IsUnicode(false);

                entity.Property(e => e.WaterTransferRegistrationStatusName).IsUnicode(false);
            });

            modelBuilder.Entity<WaterTransferType>(entity =>
            {
                entity.HasIndex(e => e.WaterTransferTypeDisplayName)
                    .HasName("AK_WaterTransferType_WaterTransferTypeDisplayName")
                    .IsUnique();

                entity.HasIndex(e => e.WaterTransferTypeName)
                    .HasName("AK_WaterTransferType_WaterTransferTypeName")
                    .IsUnique();

                entity.Property(e => e.WaterTransferTypeID).ValueGeneratedNever();

                entity.Property(e => e.WaterTransferTypeDisplayName).IsUnicode(false);

                entity.Property(e => e.WaterTransferTypeName).IsUnicode(false);
            });

            modelBuilder.Entity<vAllParcelsWithAnnualWaterUsage>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vAllParcelsWithAnnualWaterUsage");

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.FirstName).IsUnicode(false);

                entity.Property(e => e.LastName).IsUnicode(false);

                entity.Property(e => e.ParcelNumber).IsUnicode(false);
            });

            modelBuilder.Entity<vGeoServerAllParcels>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vGeoServerAllParcels");

                entity.Property(e => e.LandOwnerFullName).IsUnicode(false);

                entity.Property(e => e.OwnerAddress).IsUnicode(false);

                entity.Property(e => e.OwnerCity).IsUnicode(false);

                entity.Property(e => e.OwnerName).IsUnicode(false);

                entity.Property(e => e.ParcelNumber).IsUnicode(false);
            });

            modelBuilder.Entity<vParcelOwnership>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vParcelOwnership");

                entity.Property(e => e.Note).IsUnicode(false);

                entity.Property(e => e.OwnerName).IsUnicode(false);

                entity.Property(e => e.UserParcelID).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<vPostingDetailed>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vPostingDetailed");

                entity.Property(e => e.PostedByEmail).IsUnicode(false);

                entity.Property(e => e.PostedByFirstName).IsUnicode(false);

                entity.Property(e => e.PostedByLastName).IsUnicode(false);

                entity.Property(e => e.PostingStatusDisplayName).IsUnicode(false);

                entity.Property(e => e.PostingTypeDisplayName).IsUnicode(false);
            });

            modelBuilder.Entity<vUserDetailed>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vUserDetailed");

                entity.Property(e => e.Company).IsUnicode(false);

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.FirstName).IsUnicode(false);

                entity.Property(e => e.LastName).IsUnicode(false);

                entity.Property(e => e.LoginName).IsUnicode(false);

                entity.Property(e => e.Phone).IsUnicode(false);

                entity.Property(e => e.RoleDisplayName).IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

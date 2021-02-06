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

        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<AccountParcelWaterYear> AccountParcelWaterYear { get; set; }
        public virtual DbSet<AccountReconciliation> AccountReconciliation { get; set; }
        public virtual DbSet<AccountStatus> AccountStatus { get; set; }
        public virtual DbSet<AccountUser> AccountUser { get; set; }
        public virtual DbSet<CustomRichText> CustomRichText { get; set; }
        public virtual DbSet<CustomRichTextType> CustomRichTextType { get; set; }
        public virtual DbSet<DatabaseMigration> DatabaseMigration { get; set; }
        public virtual DbSet<DisadvantagedCommunity> DisadvantagedCommunity { get; set; }
        public virtual DbSet<DisadvantagedCommunityStatus> DisadvantagedCommunityStatus { get; set; }
        public virtual DbSet<FileResource> FileResource { get; set; }
        public virtual DbSet<FileResourceMimeType> FileResourceMimeType { get; set; }
        public virtual DbSet<Offer> Offer { get; set; }
        public virtual DbSet<OfferStatus> OfferStatus { get; set; }
        public virtual DbSet<OpenETGoogleBucketResponseEvapotranspirationData> OpenETGoogleBucketResponseEvapotranspirationData { get; set; }
        public virtual DbSet<OpenETSyncHistory> OpenETSyncHistory { get; set; }
        public virtual DbSet<OpenETSyncResultType> OpenETSyncResultType { get; set; }
        public virtual DbSet<Parcel> Parcel { get; set; }
        public virtual DbSet<ParcelAllocation> ParcelAllocation { get; set; }
        public virtual DbSet<ParcelAllocationHistory> ParcelAllocationHistory { get; set; }
        public virtual DbSet<ParcelAllocationType> ParcelAllocationType { get; set; }
        public virtual DbSet<ParcelLayerGDBCommonMappingToParcelStagingColumn> ParcelLayerGDBCommonMappingToParcelStagingColumn { get; set; }
        public virtual DbSet<ParcelMonthlyEvapotranspiration> ParcelMonthlyEvapotranspiration { get; set; }
        public virtual DbSet<ParcelStatus> ParcelStatus { get; set; }
        public virtual DbSet<ParcelUpdateStaging> ParcelUpdateStaging { get; set; }
        public virtual DbSet<Posting> Posting { get; set; }
        public virtual DbSet<PostingStatus> PostingStatus { get; set; }
        public virtual DbSet<PostingType> PostingType { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<ScenarioArsenicContaminationLocation> ScenarioArsenicContaminationLocation { get; set; }
        public virtual DbSet<ScenarioRechargeBasin> ScenarioRechargeBasin { get; set; }
        public virtual DbSet<Trade> Trade { get; set; }
        public virtual DbSet<TradeStatus> TradeStatus { get; set; }
        public virtual DbSet<UploadedGdb> UploadedGdb { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<WaterTradingScenarioWell> WaterTradingScenarioWell { get; set; }
        public virtual DbSet<WaterTransfer> WaterTransfer { get; set; }
        public virtual DbSet<WaterTransferRegistration> WaterTransferRegistration { get; set; }
        public virtual DbSet<WaterTransferRegistrationParcel> WaterTransferRegistrationParcel { get; set; }
        public virtual DbSet<WaterTransferRegistrationStatus> WaterTransferRegistrationStatus { get; set; }
        public virtual DbSet<WaterTransferType> WaterTransferType { get; set; }
        public virtual DbSet<WaterYear> WaterYear { get; set; }
        public virtual DbSet<Well> Well { get; set; }
        public virtual DbSet<geometry_columns> geometry_columns { get; set; }
        public virtual DbSet<spatial_ref_sys> spatial_ref_sys { get; set; }
        public virtual DbSet<vGeoServerAllParcels> vGeoServerAllParcels { get; set; }
        public virtual DbSet<vGeoServerDisadvantagedCommunity> vGeoServerDisadvantagedCommunity { get; set; }
        public virtual DbSet<vGeoServerScenarioArsenicContaminationLocation> vGeoServerScenarioArsenicContaminationLocation { get; set; }
        public virtual DbSet<vGeoServerScenarioRechargeBasin> vGeoServerScenarioRechargeBasin { get; set; }
        public virtual DbSet<vGeoServerWaterTradingScenarioWell> vGeoServerWaterTradingScenarioWell { get; set; }
        public virtual DbSet<vGeoServerWells> vGeoServerWells { get; set; }
        public virtual DbSet<vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry> vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry { get; set; }
        public virtual DbSet<vParcelLayerUpdateDifferencesInParcelsAssociatedWithAccount> vParcelLayerUpdateDifferencesInParcelsAssociatedWithAccount { get; set; }
        public virtual DbSet<vParcelOwnership> vParcelOwnership { get; set; }
        public virtual DbSet<vPostingDetailed> vPostingDetailed { get; set; }
        public virtual DbSet<vUserDetailed> vUserDetailed { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            {

                
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasIndex(e => e.AccountNumber)
                    .HasName("AK_Account_AccountNumber")
                    .IsUnique();

                entity.HasIndex(e => e.AccountVerificationKey)
                    .HasName("AK_Account_AccountVerificationKey")
                    .IsUnique()
                    .HasFilter("([AccountVerificationKey] IS NOT NULL)");

                entity.Property(e => e.AccountName).IsUnicode(false);

                entity.Property(e => e.AccountNumber).HasComputedColumnSql("(isnull([AccountID]+(10000),(0)))");

                entity.Property(e => e.AccountVerificationKey).IsUnicode(false);

                entity.Property(e => e.Notes).IsUnicode(false);

                entity.HasOne(d => d.AccountStatus)
                    .WithMany(p => p.Account)
                    .HasForeignKey(d => d.AccountStatusID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<AccountParcelWaterYear>(entity =>
            {
                entity.HasIndex(e => new { e.ParcelID, e.WaterYearID })
                    .HasName("AK_AccountParcelWaterYear_ParcelID_WaterYearID")
                    .IsUnique();

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AccountParcelWaterYear)
                    .HasForeignKey(d => d.AccountID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Parcel)
                    .WithMany(p => p.AccountParcelWaterYear)
                    .HasForeignKey(d => d.ParcelID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.WaterYear)
                    .WithMany(p => p.AccountParcelWaterYear)
                    .HasForeignKey(d => d.WaterYearID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<AccountReconciliation>(entity =>
            {
                entity.HasKey(e => e.AccountReconciliation1)
                    .HasName("PK_AccountReconciliation_AccountReconciliationID");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AccountReconciliation)
                    .HasForeignKey(d => d.AccountID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Parcel)
                    .WithMany(p => p.AccountReconciliation)
                    .HasForeignKey(d => d.ParcelID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<AccountStatus>(entity =>
            {
                entity.HasIndex(e => e.AccountStatusDisplayName)
                    .HasName("AK_AccountStatus_AccountStatusDisplayName")
                    .IsUnique();

                entity.HasIndex(e => e.AccountStatusName)
                    .HasName("AK_AccountStatus_AccountStatusName")
                    .IsUnique();

                entity.Property(e => e.AccountStatusID).ValueGeneratedNever();

                entity.Property(e => e.AccountStatusDisplayName).IsUnicode(false);

                entity.Property(e => e.AccountStatusName).IsUnicode(false);
            });

            modelBuilder.Entity<AccountUser>(entity =>
            {
                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AccountUser)
                    .HasForeignKey(d => d.AccountID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AccountUser)
                    .HasForeignKey(d => d.UserID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<CustomRichText>(entity =>
            {
                entity.Property(e => e.CustomRichTextContent).IsUnicode(false);

                entity.HasOne(d => d.CustomRichTextType)
                    .WithMany(p => p.CustomRichText)
                    .HasForeignKey(d => d.CustomRichTextTypeID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<CustomRichTextType>(entity =>
            {
                entity.HasIndex(e => e.CustomRichTextTypeDisplayName)
                    .HasName("AK_CustomRichTextType_CustomRichTextTypeDisplayName")
                    .IsUnique();

                entity.HasIndex(e => e.CustomRichTextTypeName)
                    .HasName("AK_CustomRichTextType_CustomRichTextTypeName")
                    .IsUnique();

                entity.Property(e => e.CustomRichTextTypeID).ValueGeneratedNever();

                entity.Property(e => e.CustomRichTextTypeDisplayName).IsUnicode(false);

                entity.Property(e => e.CustomRichTextTypeName).IsUnicode(false);
            });

            modelBuilder.Entity<DatabaseMigration>(entity =>
            {
                entity.HasKey(e => e.DatabaseMigrationNumber)
                    .HasName("PK_DatabaseMigration_DatabaseMigrationNumber");

                entity.Property(e => e.DatabaseMigrationNumber).ValueGeneratedNever();
            });

            modelBuilder.Entity<DisadvantagedCommunity>(entity =>
            {
                entity.HasIndex(e => new { e.DisadvantagedCommunityName, e.LSADCode })
                    .HasName("AK_DisadvantagedCommunity_DisadvantagedCommunityName_LSADCode")
                    .IsUnique();

                entity.Property(e => e.DisadvantagedCommunityID).ValueGeneratedNever();

                entity.Property(e => e.DisadvantagedCommunityName).IsUnicode(false);

                entity.HasOne(d => d.DisadvantagedCommunityStatus)
                    .WithMany(p => p.DisadvantagedCommunity)
                    .HasForeignKey(d => d.DisadvantagedCommunityStatusID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<DisadvantagedCommunityStatus>(entity =>
            {
                entity.HasIndex(e => e.DisadvantagedCommunityStatusName)
                    .HasName("AK_DisadvantagedCommunityStatus_DisadvantagedCommunityStatusName")
                    .IsUnique();

                entity.Property(e => e.DisadvantagedCommunityStatusID).ValueGeneratedNever();

                entity.Property(e => e.DisadvantagedCommunityStatusName).IsUnicode(false);

                entity.Property(e => e.GeoServerLayerColor).IsUnicode(false);
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

                entity.HasOne(d => d.CreateAccount)
                    .WithMany(p => p.Offer)
                    .HasForeignKey(d => d.CreateAccountID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Offer_User_CreateAccountID_AccountID");

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

            modelBuilder.Entity<OpenETGoogleBucketResponseEvapotranspirationData>(entity =>
            {
                entity.Property(e => e.ParcelNumber).IsUnicode(false);
            });

            modelBuilder.Entity<OpenETSyncHistory>(entity =>
            {
                entity.HasOne(d => d.OpenETSyncResultType)
                    .WithMany(p => p.OpenETSyncHistory)
                    .HasForeignKey(d => d.OpenETSyncResultTypeID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.WaterYear)
                    .WithMany(p => p.OpenETSyncHistory)
                    .HasForeignKey(d => d.WaterYearID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<OpenETSyncResultType>(entity =>
            {
                entity.HasIndex(e => e.OpenETSyncResultTypeDisplayName)
                    .HasName("AK_OpenETSyncResultType_OpenETSyncResultTypeDisplayName")
                    .IsUnique();

                entity.HasIndex(e => e.OpenETSyncResultTypeName)
                    .HasName("AK_OpenETSyncResultType_AK_OpenETSyncResultTypeName")
                    .IsUnique();

                entity.Property(e => e.OpenETSyncResultTypeID).ValueGeneratedNever();

                entity.Property(e => e.OpenETSyncResultTypeDisplayName).IsUnicode(false);

                entity.Property(e => e.OpenETSyncResultTypeName).IsUnicode(false);
            });

            modelBuilder.Entity<Parcel>(entity =>
            {
                entity.HasIndex(e => e.ParcelNumber)
                    .HasName("AK_Parcel_ParcelNumber")
                    .IsUnique();

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

            modelBuilder.Entity<ParcelAllocationHistory>(entity =>
            {
                entity.HasOne(d => d.ParcelAllocationType)
                    .WithMany(p => p.ParcelAllocationHistory)
                    .HasForeignKey(d => d.ParcelAllocationTypeID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ParcelAllocationHistory)
                    .HasForeignKey(d => d.UserID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ParcelAllocationType>(entity =>
            {
                entity.HasIndex(e => e.IsSourcedFromApi)
                    .HasName("CK_ParcelAllocationType_AtMostOne_IsSourcedFromApi_True")
                    .IsUnique()
                    .HasFilter("([IsSourcedFromApi]=(1))");

                entity.HasIndex(e => e.ParcelAllocationTypeName)
                    .HasName("AK_ParcelAllocationType_ParcelAllocationTypeName")
                    .IsUnique();

                entity.Property(e => e.ParcelAllocationTypeDefinition).IsUnicode(false);

                entity.Property(e => e.ParcelAllocationTypeName).IsUnicode(false);
            });

            modelBuilder.Entity<ParcelLayerGDBCommonMappingToParcelStagingColumn>(entity =>
            {
                entity.HasKey(e => e.ParcelLayerGDBCommonMappingToParcelColumnID)
                    .HasName("PK_ParcelLayerGDBCommonMappingToParcelColumn_ParcelLayerGDBCommonMappingToParcelColumnID");

                entity.HasIndex(e => e.OwnerName)
                    .HasName("AK_ParcelLayerGDBCommonMappingToParcelColumn_OwnerName")
                    .IsUnique();

                entity.HasIndex(e => e.ParcelNumber)
                    .HasName("AK_ParcelLayerGDBCommonMappingToParcelColumn_ParcelNumber")
                    .IsUnique();

                entity.Property(e => e.OwnerName).IsUnicode(false);

                entity.Property(e => e.ParcelNumber).IsUnicode(false);
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

            modelBuilder.Entity<ParcelStatus>(entity =>
            {
                entity.HasIndex(e => e.ParcelStatusDisplayName)
                    .HasName("AK_ParcelStatus_ParcelStatusDisplayName")
                    .IsUnique();

                entity.HasIndex(e => e.ParcelStatusName)
                    .HasName("AK_ParcelStatus_ParcelStatusName")
                    .IsUnique();

                entity.Property(e => e.ParcelStatusID).ValueGeneratedNever();

                entity.Property(e => e.ParcelStatusDisplayName).IsUnicode(false);

                entity.Property(e => e.ParcelStatusName).IsUnicode(false);
            });

            modelBuilder.Entity<ParcelUpdateStaging>(entity =>
            {
                entity.Property(e => e.OwnerName).IsUnicode(false);

                entity.Property(e => e.ParcelGeometry4326Text).IsUnicode(false);

                entity.Property(e => e.ParcelGeometryText).IsUnicode(false);

                entity.Property(e => e.ParcelNumber).IsUnicode(false);
            });

            modelBuilder.Entity<Posting>(entity =>
            {
                entity.Property(e => e.PostingDescription).IsUnicode(false);

                entity.HasOne(d => d.CreateAccount)
                    .WithMany(p => p.Posting)
                    .HasForeignKey(d => d.CreateAccountID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Posting_Account_CreateAccountID_AccountID");

                entity.HasOne(d => d.CreateUser)
                    .WithMany(p => p.Posting)
                    .HasForeignKey(d => d.CreateUserID)
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

            modelBuilder.Entity<ScenarioArsenicContaminationLocation>(entity =>
            {
                entity.HasIndex(e => e.ScenarioArsenicContaminationLocationWellName)
                    .HasName("AK_ScenarioArsenicContaminationLocation_ScenarioArsenicContaminationLocationWellName")
                    .IsUnique();

                entity.Property(e => e.ScenarioArsenicContaminationLocationWellName).IsUnicode(false);
            });

            modelBuilder.Entity<ScenarioRechargeBasin>(entity =>
            {
                entity.HasIndex(e => new { e.ScenarioRechargeBasinName, e.ScenarioRechargeBasinDisplayName })
                    .HasName("AK_ScenarioRechargeBasin_ScenarioRechargeBasinName_ScenarioRechargeBasinDisplayName")
                    .IsUnique();

                entity.Property(e => e.ScenarioRechargeBasinID).ValueGeneratedNever();

                entity.Property(e => e.ScenarioRechargeBasinDisplayName).IsUnicode(false);

                entity.Property(e => e.ScenarioRechargeBasinName).IsUnicode(false);
            });

            modelBuilder.Entity<Trade>(entity =>
            {
                entity.Property(e => e.TradeNumber).IsUnicode(false);

                entity.HasOne(d => d.CreateAccount)
                    .WithMany(p => p.Trade)
                    .HasForeignKey(d => d.CreateAccountID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Trade_Account_CreateAccountID_AccountID");

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

            modelBuilder.Entity<UploadedGdb>(entity =>
            {
                entity.Property(e => e.UploadedGdbID).ValueGeneratedNever();
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

            modelBuilder.Entity<WaterTradingScenarioWell>(entity =>
            {
                entity.Property(e => e.WaterTradingScenarioWellID).ValueGeneratedNever();

                entity.Property(e => e.WaterTradingScenarioWellCountyName).IsUnicode(false);
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
                entity.HasOne(d => d.Account)
                    .WithMany(p => p.WaterTransferRegistration)
                    .HasForeignKey(d => d.AccountID)
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

            modelBuilder.Entity<WaterYear>(entity =>
            {
                entity.HasIndex(e => e.Year)
                    .HasName("AK_WaterYear_Year")
                    .IsUnique();
            });

            modelBuilder.Entity<Well>(entity =>
            {
                entity.Property(e => e.WellID).ValueGeneratedNever();

                entity.Property(e => e.WellName).IsUnicode(false);

                entity.Property(e => e.WellType).IsUnicode(false);

                entity.Property(e => e.WellTypeCodeName).IsUnicode(false);
            });

            modelBuilder.Entity<geometry_columns>(entity =>
            {
                entity.HasKey(e => new { e.f_table_catalog, e.f_table_schema, e.f_table_name, e.f_geometry_column })
                    .HasName("geometry_columns_pk");

                entity.Property(e => e.f_table_catalog).IsUnicode(false);

                entity.Property(e => e.f_table_schema).IsUnicode(false);

                entity.Property(e => e.f_table_name).IsUnicode(false);

                entity.Property(e => e.f_geometry_column).IsUnicode(false);

                entity.Property(e => e.geometry_type).IsUnicode(false);
            });

            modelBuilder.Entity<spatial_ref_sys>(entity =>
            {
                entity.HasKey(e => e.srid)
                    .HasName("PK__spatial___36B11BD545349F5B");

                entity.Property(e => e.srid).ValueGeneratedNever();

                entity.Property(e => e.auth_name).IsUnicode(false);

                entity.Property(e => e.proj4text).IsUnicode(false);

                entity.Property(e => e.srtext).IsUnicode(false);
            });

            modelBuilder.Entity<vGeoServerAllParcels>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vGeoServerAllParcels");

                entity.Property(e => e.ParcelNumber).IsUnicode(false);
            });

            modelBuilder.Entity<vGeoServerDisadvantagedCommunity>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vGeoServerDisadvantagedCommunity");

                entity.Property(e => e.DisadvantagedCommunityName).IsUnicode(false);

                entity.Property(e => e.DisadvantagedCommunityStatusName).IsUnicode(false);

                entity.Property(e => e.GeoServerLayerColor).IsUnicode(false);
            });

            modelBuilder.Entity<vGeoServerScenarioArsenicContaminationLocation>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vGeoServerScenarioArsenicContaminationLocation");

                entity.Property(e => e.PrimaryKey).ValueGeneratedOnAdd();

                entity.Property(e => e.ScenarioArsenicContaminationLocationWellName).IsUnicode(false);
            });

            modelBuilder.Entity<vGeoServerScenarioRechargeBasin>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vGeoServerScenarioRechargeBasin");

                entity.Property(e => e.ScenarioRechargeBasinDisplayName).IsUnicode(false);

                entity.Property(e => e.ScenarioRechargeBasinName).IsUnicode(false);
            });

            modelBuilder.Entity<vGeoServerWaterTradingScenarioWell>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vGeoServerWaterTradingScenarioWell");

                entity.Property(e => e.WaterTradingScenarioWellCountyName).IsUnicode(false);
            });

            modelBuilder.Entity<vGeoServerWells>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vGeoServerWells");

                entity.Property(e => e.WellName).IsUnicode(false);
            });

            modelBuilder.Entity<vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry");

                entity.Property(e => e.NewOwnerName).IsUnicode(false);

                entity.Property(e => e.OldOwnerName).IsUnicode(false);

                entity.Property(e => e.ParcelNumber).IsUnicode(false);
            });

            modelBuilder.Entity<vParcelLayerUpdateDifferencesInParcelsAssociatedWithAccount>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vParcelLayerUpdateDifferencesInParcelsAssociatedWithAccount");

                entity.Property(e => e.AccountName).IsUnicode(false);

                entity.Property(e => e.ExistingParcels).IsUnicode(false);

                entity.Property(e => e.UpdatedParcels).IsUnicode(false);
            });

            modelBuilder.Entity<vParcelOwnership>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vParcelOwnership");
            });

            modelBuilder.Entity<vPostingDetailed>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vPostingDetailed");

                entity.Property(e => e.PostedByAccountName).IsUnicode(false);

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

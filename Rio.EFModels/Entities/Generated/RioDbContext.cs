using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

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

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<AccountParcelWaterYear> AccountParcelWaterYears { get; set; }
        public virtual DbSet<AccountReconciliation> AccountReconciliations { get; set; }
        public virtual DbSet<AccountUser> AccountUsers { get; set; }
        public virtual DbSet<CustomRichText> CustomRichTexts { get; set; }
        public virtual DbSet<DisadvantagedCommunity> DisadvantagedCommunities { get; set; }
        public virtual DbSet<DisadvantagedCommunityStatus> DisadvantagedCommunityStatuses { get; set; }
        public virtual DbSet<FileResource> FileResources { get; set; }
        public virtual DbSet<Offer> Offers { get; set; }
        public virtual DbSet<OpenETGoogleBucketResponseEvapotranspirationDatum> OpenETGoogleBucketResponseEvapotranspirationData { get; set; }
        public virtual DbSet<OpenETSyncHistory> OpenETSyncHistories { get; set; }
        public virtual DbSet<Parcel> Parcels { get; set; }
        public virtual DbSet<ParcelLayerGDBCommonMappingToParcelStagingColumn> ParcelLayerGDBCommonMappingToParcelStagingColumns { get; set; }
        public virtual DbSet<ParcelLedger> ParcelLedgers { get; set; }
        public virtual DbSet<ParcelTag> ParcelTags { get; set; }
        public virtual DbSet<ParcelUpdateStaging> ParcelUpdateStagings { get; set; }
        public virtual DbSet<Posting> Postings { get; set; }
        public virtual DbSet<ScenarioArsenicContaminationLocation> ScenarioArsenicContaminationLocations { get; set; }
        public virtual DbSet<ScenarioRechargeBasin> ScenarioRechargeBasins { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<Trade> Trades { get; set; }
        public virtual DbSet<UploadedGdb> UploadedGdbs { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<WaterTradingScenarioWell> WaterTradingScenarioWells { get; set; }
        public virtual DbSet<WaterTransfer> WaterTransfers { get; set; }
        public virtual DbSet<WaterTransferRegistration> WaterTransferRegistrations { get; set; }
        public virtual DbSet<WaterTransferRegistrationParcel> WaterTransferRegistrationParcels { get; set; }
        public virtual DbSet<WaterType> WaterTypes { get; set; }
        public virtual DbSet<WaterYear> WaterYears { get; set; }
        public virtual DbSet<WaterYearMonth> WaterYearMonths { get; set; }
        public virtual DbSet<Well> Wells { get; set; }
        public virtual DbSet<geometry_column> geometry_columns { get; set; }
        public virtual DbSet<spatial_ref_sy> spatial_ref_sys { get; set; }
        public virtual DbSet<vOpenETMostRecentSyncHistoryForYearAndMonth> vOpenETMostRecentSyncHistoryForYearAndMonths { get; set; }
        public virtual DbSet<vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry> vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometries { get; set; }
        public virtual DbSet<vParcelLayerUpdateDifferencesInParcelsAssociatedWithAccount> vParcelLayerUpdateDifferencesInParcelsAssociatedWithAccounts { get; set; }
        public virtual DbSet<vParcelOwnership> vParcelOwnerships { get; set; }
        public virtual DbSet<vPostingDetailed> vPostingDetaileds { get; set; }
        public virtual DbSet<vUserDetailed> vUserDetaileds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasIndex(e => e.AccountVerificationKey, "AK_Account_AccountVerificationKey")
                    .IsUnique()
                    .HasFilter("([AccountVerificationKey] IS NOT NULL)");

                entity.Property(e => e.AccountName).IsUnicode(false);

                entity.Property(e => e.AccountNumber).HasComputedColumnSql("(isnull([AccountID]+(10000),(0)))", false);

                entity.Property(e => e.AccountVerificationKey).IsUnicode(false);

                entity.Property(e => e.Notes).IsUnicode(false);
            });

            modelBuilder.Entity<AccountParcelWaterYear>(entity =>
            {
                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AccountParcelWaterYears)
                    .HasForeignKey(d => d.AccountID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Parcel)
                    .WithMany(p => p.AccountParcelWaterYears)
                    .HasForeignKey(d => d.ParcelID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.WaterYear)
                    .WithMany(p => p.AccountParcelWaterYears)
                    .HasForeignKey(d => d.WaterYearID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<AccountReconciliation>(entity =>
            {
                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AccountReconciliations)
                    .HasForeignKey(d => d.AccountID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Parcel)
                    .WithMany(p => p.AccountReconciliations)
                    .HasForeignKey(d => d.ParcelID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<AccountUser>(entity =>
            {
                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AccountUsers)
                    .HasForeignKey(d => d.AccountID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AccountUsers)
                    .HasForeignKey(d => d.UserID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<CustomRichText>(entity =>
            {
                entity.Property(e => e.CustomRichTextContent).IsUnicode(false);
            });

            modelBuilder.Entity<DisadvantagedCommunity>(entity =>
            {
                entity.Property(e => e.DisadvantagedCommunityID).ValueGeneratedNever();

                entity.Property(e => e.DisadvantagedCommunityName).IsUnicode(false);

                entity.HasOne(d => d.DisadvantagedCommunityStatus)
                    .WithMany(p => p.DisadvantagedCommunities)
                    .HasForeignKey(d => d.DisadvantagedCommunityStatusID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<DisadvantagedCommunityStatus>(entity =>
            {
                entity.Property(e => e.DisadvantagedCommunityStatusID).ValueGeneratedNever();

                entity.Property(e => e.DisadvantagedCommunityStatusName).IsUnicode(false);

                entity.Property(e => e.GeoServerLayerColor).IsUnicode(false);
            });

            modelBuilder.Entity<FileResource>(entity =>
            {
                entity.Property(e => e.OriginalBaseFilename).IsUnicode(false);

                entity.Property(e => e.OriginalFileExtension).IsUnicode(false);

                entity.HasOne(d => d.CreateUser)
                    .WithMany(p => p.FileResources)
                    .HasForeignKey(d => d.CreateUserID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FileResource_User_CreateUserID_UserID");
            });

            modelBuilder.Entity<Offer>(entity =>
            {
                entity.Property(e => e.OfferNotes).IsUnicode(false);

                entity.HasOne(d => d.CreateAccount)
                    .WithMany(p => p.Offers)
                    .HasForeignKey(d => d.CreateAccountID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Offer_User_CreateAccountID_AccountID");

                entity.HasOne(d => d.Trade)
                    .WithMany(p => p.Offers)
                    .HasForeignKey(d => d.TradeID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<OpenETGoogleBucketResponseEvapotranspirationDatum>(entity =>
            {
                entity.HasKey(e => e.OpenETGoogleBucketResponseEvapotranspirationDataID)
                    .HasName("PK_OpenETGoogleBucketResponseEvapotranspirationData_OpenETGoogleBucketResponseEvapotranspirationDataID");

                entity.Property(e => e.ParcelNumber).IsUnicode(false);
            });

            modelBuilder.Entity<OpenETSyncHistory>(entity =>
            {
                entity.Property(e => e.ErrorMessage).IsUnicode(false);

                entity.Property(e => e.GoogleBucketFileRetrievalURL).IsUnicode(false);

                entity.HasOne(d => d.WaterYearMonth)
                    .WithMany(p => p.OpenETSyncHistories)
                    .HasForeignKey(d => d.WaterYearMonthID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Parcel>(entity =>
            {
                entity.Property(e => e.ParcelNumber).IsUnicode(false);
            });

            modelBuilder.Entity<ParcelLayerGDBCommonMappingToParcelStagingColumn>(entity =>
            {
                entity.HasKey(e => e.ParcelLayerGDBCommonMappingToParcelColumnID)
                    .HasName("PK_ParcelLayerGDBCommonMappingToParcelColumn_ParcelLayerGDBCommonMappingToParcelColumnID");

                entity.Property(e => e.OwnerName).IsUnicode(false);

                entity.Property(e => e.ParcelNumber).IsUnicode(false);
            });

            modelBuilder.Entity<ParcelLedger>(entity =>
            {
                entity.Property(e => e.TransactionDescription).IsUnicode(false);

                entity.Property(e => e.UploadedFileName).IsUnicode(false);

                entity.Property(e => e.UserComment).IsUnicode(false);

                entity.HasOne(d => d.Parcel)
                    .WithMany(p => p.ParcelLedgers)
                    .HasForeignKey(d => d.ParcelID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ParcelTag>(entity =>
            {
                entity.HasOne(d => d.Parcel)
                    .WithMany(p => p.ParcelTags)
                    .HasForeignKey(d => d.ParcelID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.ParcelTags)
                    .HasForeignKey(d => d.TagID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
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
                    .WithMany(p => p.Postings)
                    .HasForeignKey(d => d.CreateAccountID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Posting_Account_CreateAccountID_AccountID");

                entity.HasOne(d => d.CreateUser)
                    .WithMany(p => p.Postings)
                    .HasForeignKey(d => d.CreateUserID)
                    .HasConstraintName("FK_Posting_User_CreateUserID_UserID");
            });

            modelBuilder.Entity<ScenarioArsenicContaminationLocation>(entity =>
            {
                entity.Property(e => e.ScenarioArsenicContaminationLocationWellName).IsUnicode(false);
            });

            modelBuilder.Entity<ScenarioRechargeBasin>(entity =>
            {
                entity.Property(e => e.ScenarioRechargeBasinID).ValueGeneratedNever();

                entity.Property(e => e.ScenarioRechargeBasinDisplayName).IsUnicode(false);

                entity.Property(e => e.ScenarioRechargeBasinName).IsUnicode(false);
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.Property(e => e.TagDescription).IsUnicode(false);

                entity.Property(e => e.TagName).IsUnicode(false);
            });

            modelBuilder.Entity<Trade>(entity =>
            {
                entity.Property(e => e.TradeNumber).IsUnicode(false);

                entity.HasOne(d => d.CreateAccount)
                    .WithMany(p => p.Trades)
                    .HasForeignKey(d => d.CreateAccountID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Trade_Account_CreateAccountID_AccountID");

                entity.HasOne(d => d.Posting)
                    .WithMany(p => p.Trades)
                    .HasForeignKey(d => d.PostingID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<UploadedGdb>(entity =>
            {
                entity.Property(e => e.UploadedGdbID).ValueGeneratedNever();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.UserGuid, "AK_User_UserGuid")
                    .IsUnique()
                    .HasFilter("([UserGuid] IS NOT NULL)");

                entity.Property(e => e.Company).IsUnicode(false);

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.FirstName).IsUnicode(false);

                entity.Property(e => e.LastName).IsUnicode(false);

                entity.Property(e => e.LoginName).IsUnicode(false);

                entity.Property(e => e.Phone).IsUnicode(false);
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
                    .WithMany(p => p.WaterTransfers)
                    .HasForeignKey(d => d.OfferID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<WaterTransferRegistration>(entity =>
            {
                entity.HasOne(d => d.Account)
                    .WithMany(p => p.WaterTransferRegistrations)
                    .HasForeignKey(d => d.AccountID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.WaterTransfer)
                    .WithMany(p => p.WaterTransferRegistrations)
                    .HasForeignKey(d => d.WaterTransferID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<WaterTransferRegistrationParcel>(entity =>
            {
                entity.HasOne(d => d.Parcel)
                    .WithMany(p => p.WaterTransferRegistrationParcels)
                    .HasForeignKey(d => d.ParcelID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.WaterTransferRegistration)
                    .WithMany(p => p.WaterTransferRegistrationParcels)
                    .HasForeignKey(d => d.WaterTransferRegistrationID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<WaterType>(entity =>
            {
                entity.Property(e => e.WaterTypeDefinition).IsUnicode(false);

                entity.Property(e => e.WaterTypeName).IsUnicode(false);
            });

            modelBuilder.Entity<WaterYearMonth>(entity =>
            {
                entity.HasOne(d => d.WaterYear)
                    .WithMany(p => p.WaterYearMonths)
                    .HasForeignKey(d => d.WaterYearID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Well>(entity =>
            {
                entity.Property(e => e.WellID).ValueGeneratedNever();

                entity.Property(e => e.WellName).IsUnicode(false);

                entity.Property(e => e.WellType).IsUnicode(false);

                entity.Property(e => e.WellTypeCodeName).IsUnicode(false);
            });

            modelBuilder.Entity<geometry_column>(entity =>
            {
                entity.HasKey(e => new { e.f_table_catalog, e.f_table_schema, e.f_table_name, e.f_geometry_column })
                    .HasName("geometry_columns_pk");

                entity.Property(e => e.f_table_catalog).IsUnicode(false);

                entity.Property(e => e.f_table_schema).IsUnicode(false);

                entity.Property(e => e.f_table_name).IsUnicode(false);

                entity.Property(e => e.f_geometry_column).IsUnicode(false);

                entity.Property(e => e.geometry_type).IsUnicode(false);
            });

            modelBuilder.Entity<spatial_ref_sy>(entity =>
            {
                entity.HasKey(e => e.srid)
                    .HasName("PK_spatial_ref_sys_srid");

                entity.Property(e => e.srid).ValueGeneratedNever();

                entity.Property(e => e.auth_name).IsUnicode(false);

                entity.Property(e => e.proj4text).IsUnicode(false);

                entity.Property(e => e.srtext).IsUnicode(false);
            });

            modelBuilder.Entity<vOpenETMostRecentSyncHistoryForYearAndMonth>(entity =>
            {
                entity.ToView("vOpenETMostRecentSyncHistoryForYearAndMonth");

                entity.Property(e => e.ErrorMessage).IsUnicode(false);

                entity.Property(e => e.GoogleBucketFileRetrievalURL).IsUnicode(false);
            });

            modelBuilder.Entity<vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry>(entity =>
            {
                entity.ToView("vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry");

                entity.Property(e => e.NewOwnerName).IsUnicode(false);

                entity.Property(e => e.OldOwnerName).IsUnicode(false);

                entity.Property(e => e.ParcelNumber).IsUnicode(false);
            });

            modelBuilder.Entity<vParcelLayerUpdateDifferencesInParcelsAssociatedWithAccount>(entity =>
            {
                entity.ToView("vParcelLayerUpdateDifferencesInParcelsAssociatedWithAccount");

                entity.Property(e => e.AccountName).IsUnicode(false);

                entity.Property(e => e.ExistingParcels).IsUnicode(false);

                entity.Property(e => e.UpdatedParcels).IsUnicode(false);
            });

            modelBuilder.Entity<vParcelOwnership>(entity =>
            {
                entity.ToView("vParcelOwnership");
            });

            modelBuilder.Entity<vPostingDetailed>(entity =>
            {
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

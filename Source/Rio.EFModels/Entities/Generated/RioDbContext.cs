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

        public virtual DbSet<AuditLog> AuditLog { get; set; }
        public virtual DbSet<AuditLogEventType> AuditLogEventType { get; set; }
        public virtual DbSet<RioPage> RioPage { get; set; }
        public virtual DbSet<RioPageImage> RioPageImage { get; set; }
        public virtual DbSet<RioPageType> RioPageType { get; set; }
        public virtual DbSet<FieldDefinition> FieldDefinition { get; set; }
        public virtual DbSet<FieldDefinitionData> FieldDefinitionData { get; set; }
        public virtual DbSet<FieldDefinitionDataImage> FieldDefinitionDataImage { get; set; }
        public virtual DbSet<FileResource> FileResource { get; set; }
        public virtual DbSet<FileResourceMimeType> FileResourceMimeType { get; set; }
        public virtual DbSet<Organization> Organization { get; set; }
        public virtual DbSet<OrganizationType> OrganizationType { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<SupportRequestLog> SupportRequestLog { get; set; }
        public virtual DbSet<SupportRequestType> SupportRequestType { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.Property(e => e.AuditDescription).IsUnicode(false);

                entity.Property(e => e.ColumnName).IsUnicode(false);

                entity.Property(e => e.NewValue).IsUnicode(false);

                entity.Property(e => e.OriginalValue).IsUnicode(false);

                entity.Property(e => e.TableName).IsUnicode(false);

                entity.HasOne(d => d.AuditLogEventType)
                    .WithMany(p => p.AuditLog)
                    .HasForeignKey(d => d.AuditLogEventTypeID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AuditLog)
                    .HasForeignKey(d => d.UserID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<AuditLogEventType>(entity =>
            {
                entity.HasIndex(e => e.AuditLogEventTypeDisplayName)
                    .HasName("AK_AuditLogEventType_AuditLogEventTypeDisplayName")
                    .IsUnique();

                entity.HasIndex(e => e.AuditLogEventTypeName)
                    .HasName("AK_AuditLogEventType_AuditLogEventTypeName")
                    .IsUnique();

                entity.Property(e => e.AuditLogEventTypeId).ValueGeneratedNever();

                entity.Property(e => e.AuditLogEventTypeDisplayName).IsUnicode(false);

                entity.Property(e => e.AuditLogEventTypeName).IsUnicode(false);
            });

            modelBuilder.Entity<RioPage>(entity =>
            {
                entity.HasIndex(e => e.RioPageTypeId)
                    .HasName("AK_RioPage_RioPageTypeID")
                    .IsUnique();

                entity.Property(e => e.RioPageContent).IsUnicode(false);
            });

            modelBuilder.Entity<RioPageImage>(entity =>
            {
                entity.HasIndex(e => new { e.RioPageImageId, e.FileResourceId })
                    .HasName("AK_RioPageImage_RioPageImageID_FileResourceID")
                    .IsUnique();

                entity.HasOne(d => d.RioPage)
                    .WithMany(p => p.RioPageImage)
                    .HasForeignKey(d => d.RioPageId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.FileResource)
                    .WithMany(p => p.RioPageImage)
                    .HasForeignKey(d => d.FileResourceId)
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

                entity.Property(e => e.RioPageTypeId).ValueGeneratedNever();

                entity.Property(e => e.RioPageTypeDisplayName).IsUnicode(false);

                entity.Property(e => e.RioPageTypeName).IsUnicode(false);
            });

            modelBuilder.Entity<FieldDefinition>(entity =>
            {
                entity.HasIndex(e => e.FieldDefinitionDisplayName)
                    .HasName("AK_FieldDefinition_FieldDefinitionDisplayName")
                    .IsUnique();

                entity.HasIndex(e => e.FieldDefinitionName)
                    .HasName("AK_FieldDefinition_FieldDefinitionName")
                    .IsUnique();

                entity.Property(e => e.FieldDefinitionId).ValueGeneratedNever();

                entity.Property(e => e.DefaultDefinition).IsUnicode(false);

                entity.Property(e => e.FieldDefinitionDisplayName).IsUnicode(false);

                entity.Property(e => e.FieldDefinitionName).IsUnicode(false);
            });

            modelBuilder.Entity<FieldDefinitionData>(entity =>
            {
                entity.HasIndex(e => e.FieldDefinitionId)
                    .HasName("AK_FieldDefinitionData_FieldDefinitionID")
                    .IsUnique();

                entity.Property(e => e.FieldDefinitionDataValue).IsUnicode(false);

                entity.Property(e => e.FieldDefinitionLabel).IsUnicode(false);

                entity.HasOne(d => d.FieldDefinition)
                    .WithOne(p => p.FieldDefinitionData)
                    .HasForeignKey<FieldDefinitionData>(d => d.FieldDefinitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<FieldDefinitionDataImage>(entity =>
            {
                entity.HasOne(d => d.FieldDefinitionData)
                    .WithMany(p => p.FieldDefinitionDataImage)
                    .HasForeignKey(d => d.FieldDefinitionDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.FileResource)
                    .WithMany(p => p.FieldDefinitionDataImage)
                    .HasForeignKey(d => d.FileResourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<FileResource>(entity =>
            {
                entity.HasIndex(e => e.FileResourceGuid)
                    .HasName("AK_FileResource_FileResourceGUID")
                    .IsUnique();

                entity.Property(e => e.OriginalBaseFilename).IsUnicode(false);

                entity.Property(e => e.OriginalFileExtension).IsUnicode(false);

                entity.HasOne(d => d.CreateUser)
                    .WithMany(p => p.FileResource)
                    .HasForeignKey(d => d.CreateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FileResource_User_CreateUserID_UserID");

                entity.HasOne(d => d.FileResourceMimeType)
                    .WithMany(p => p.FileResource)
                    .HasForeignKey(d => d.FileResourceMimeTypeId)
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

                entity.Property(e => e.FileResourceMimeTypeId).ValueGeneratedNever();

                entity.Property(e => e.FileResourceMimeTypeContentTypeName).IsUnicode(false);

                entity.Property(e => e.FileResourceMimeTypeDisplayName).IsUnicode(false);

                entity.Property(e => e.FileResourceMimeTypeIconNormalFilename).IsUnicode(false);

                entity.Property(e => e.FileResourceMimeTypeIconSmallFilename).IsUnicode(false);

                entity.Property(e => e.FileResourceMimeTypeName).IsUnicode(false);
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.HasIndex(e => e.OrganizationGuid)
                    .HasName("AK_Organization_OrganizationGuid")
                    .IsUnique()
                    .HasFilter("([OrganizationGuid] IS NOT NULL)");

                entity.HasIndex(e => e.OrganizationName)
                    .HasName("AK_Organization_OrganizationName")
                    .IsUnique();

                entity.Property(e => e.OrganizationName).IsUnicode(false);

                entity.Property(e => e.OrganizationShortName).IsUnicode(false);

                entity.Property(e => e.OrganizationUrl).IsUnicode(false);

                entity.HasOne(d => d.LogoFileResource)
                    .WithMany(p => p.Organization)
                    .HasForeignKey(d => d.LogoFileResourceId)
                    .HasConstraintName("FK_Organization_FileResource_LogoFileResourceID_FileResourceID");

                entity.HasOne(d => d.OrganizationType)
                    .WithMany(p => p.Organization)
                    .HasForeignKey(d => d.OrganizationTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.PrimaryContactUser)
                    .WithMany(p => p.Organization)
                    .HasForeignKey(d => d.PrimaryContactUserId)
                    .HasConstraintName("FK_Organization_User_PrimaryContactUserID_UserID");
            });

            modelBuilder.Entity<OrganizationType>(entity =>
            {
                entity.HasIndex(e => e.OrganizationTypeName)
                    .HasName("AK_OrganizationType_OrganizationTypeName")
                    .IsUnique();

                entity.Property(e => e.OrganizationTypeId).ValueGeneratedNever();

                entity.Property(e => e.LegendColor).IsUnicode(false);

                entity.Property(e => e.OrganizationTypeAbbreviation).IsUnicode(false);

                entity.Property(e => e.OrganizationTypeName).IsUnicode(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email)
                    .HasName("AK_User_Email")
                    .IsUnique();

                entity.HasIndex(e => e.UserGuid)
                    .HasName("AK_User_UserGuid")
                    .IsUnique();

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.FirstName).IsUnicode(false);

                entity.Property(e => e.LastName).IsUnicode(false);

                entity.Property(e => e.LoginName).IsUnicode(false);

                entity.Property(e => e.Phone).IsUnicode(false);

                entity.HasOne(d => d.OrganizationNavigation)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.OrganizationID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.RoleID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
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

            modelBuilder.Entity<SupportRequestLog>(entity =>
            {
                entity.Property(e => e.RequestDescription).IsUnicode(false);

                entity.Property(e => e.RequestUserEmail).IsUnicode(false);

                entity.Property(e => e.RequestUserName).IsUnicode(false);

                entity.Property(e => e.RequestUserOrganization).IsUnicode(false);

                entity.Property(e => e.RequestUserPhone).IsUnicode(false);

                entity.HasOne(d => d.RequestUser)
                    .WithMany(p => p.SupportRequestLog)
                    .HasForeignKey(d => d.RequestUserId)
                    .HasConstraintName("FK_SupportRequestLog_User_RequestUserID_UserID");

                entity.HasOne(d => d.SupportRequestType)
                    .WithMany(p => p.SupportRequestLog)
                    .HasForeignKey(d => d.SupportRequestTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<SupportRequestType>(entity =>
            {
                entity.HasIndex(e => e.SupportRequestTypeDisplayName)
                    .HasName("AK_SupportRequestType_SupportRequestTypeDisplayName")
                    .IsUnique();

                entity.HasIndex(e => e.SupportRequestTypeName)
                    .HasName("AK_SupportRequestType_SupportRequestTypeName")
                    .IsUnique();

                entity.Property(e => e.SupportRequestTypeId).ValueGeneratedNever();

                entity.Property(e => e.SupportRequestTypeDisplayName).IsUnicode(false);

                entity.Property(e => e.SupportRequestTypeName).IsUnicode(false);
            });
        }
    }
}

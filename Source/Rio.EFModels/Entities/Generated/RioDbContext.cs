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
        public virtual DbSet<Parcel> Parcel { get; set; }
        public virtual DbSet<RioPage> RioPage { get; set; }
        public virtual DbSet<RioPageImage> RioPageImage { get; set; }
        public virtual DbSet<RioPageType> RioPageType { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserParcel> UserParcel { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

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
                entity.HasIndex(e => new { e.UserID, e.ParcelID })
                    .HasName("AK_UserParcel_UserID_ParcelID")
                    .IsUnique();

                entity.HasOne(d => d.Parcel)
                    .WithMany(p => p.UserParcel)
                    .HasForeignKey(d => d.ParcelID)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserParcel)
                    .HasForeignKey(d => d.UserID)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace hethongquanlyluutrufiles.Models
{
    public partial class hethongquanlyluutrufileContext : DbContext
    {
        public hethongquanlyluutrufileContext()
        {
        }

        public hethongquanlyluutrufileContext(DbContextOptions<hethongquanlyluutrufileContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<File> Files { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<SharedFile> SharedFiles { get; set; }
        public virtual DbSet<TypeFile> TypeFiles { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=MSI;Database=hethongquanlyluutrufile;User Id=sa;Password=0966314211;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Department");

                entity.Property(e => e.DepartmentId)
                    .ValueGeneratedNever()
                    .HasColumnName("DepartmentID");

                entity.Property(e => e.DepartmentName).HasMaxLength(50);
            });

            modelBuilder.Entity<File>(entity =>
            {
                entity.Property(e => e.FileId).HasColumnName("FileID");

                entity.Property(e => e.DateUploaded).HasColumnType("datetime");

                entity.Property(e => e.FileName).HasMaxLength(100);

                entity.Property(e => e.FilePath)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.TypeFileId).HasColumnName("TypeFileID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.TypeFile)
                    .WithMany(p => p.Files)
                    .HasForeignKey(d => d.TypeFileId)
                    .HasConstraintName("FK_Files_TypeFile1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Files)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Files_Users");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.RoleId)
                    .ValueGeneratedNever()
                    .HasColumnName("RoleID");

                entity.Property(e => e.TypeRole).HasMaxLength(50);
            });

            modelBuilder.Entity<SharedFile>(entity =>
            {
                entity.Property(e => e.SharedFileId).HasColumnName("SharedFileID");

                entity.Property(e => e.DateShared).HasColumnType("datetime");

                entity.Property(e => e.FileId).HasColumnName("FileID");

                entity.Property(e => e.Message).HasColumnType("ntext");

                entity.Property(e => e.SharedWithUserId).HasColumnName("SharedWithUserID");

                entity.HasOne(d => d.File)
                    .WithMany(p => p.SharedFiles)
                    .HasForeignKey(d => d.FileId)
                    .HasConstraintName("FK_SharedFiles_Files");

                entity.HasOne(d => d.SharedWithUser)
                    .WithMany(p => p.SharedFiles)
                    .HasForeignKey(d => d.SharedWithUserId)
                    .HasConstraintName("FK_SharedFiles_Users2");
            });

            modelBuilder.Entity<TypeFile>(entity =>
            {
                entity.ToTable("TypeFile");

                entity.Property(e => e.TypeFileId)
                    .ValueGeneratedNever()
                    .HasColumnName("TypeFileID");

                entity.Property(e => e.TypeFileName)
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.AbleShared).HasDefaultValueSql("((1))");

                entity.Property(e => e.Address).HasMaxLength(100);

                entity.Property(e => e.Birthday).HasColumnType("datetime");

                entity.Property(e => e.Cmnd)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("CMND")
                    .IsFixedLength(true);

                entity.Property(e => e.Company).HasMaxLength(50);

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.Salt)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Sex).HasMaxLength(10);

                entity.Property(e => e.Typework)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.Username).HasMaxLength(50);

                entity.Property(e => e.Workplace).HasMaxLength(100);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_Users_Department1");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_Users_Role1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

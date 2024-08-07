using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Yamaha_yte.Models
{
    public partial class YAMAHA_YTEContext : DbContext
    {
        public YAMAHA_YTEContext()
        {
        }

        public YAMAHA_YTEContext(DbContextOptions<YAMAHA_YTEContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Accident> Accidents { get; set; } = null!;
        public virtual DbSet<Drug> Drugs { get; set; } = null!;
        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<Injury> Injuries { get; set; } = null!;
        public virtual DbSet<Prescription> Prescriptions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=UNI008-PC\\HE151423;Initial Catalog=YAMAHA_YTE;Persist Security Info=True;User ID=sa;Password=Cuong3110;Trust Server Certificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Accident>(entity =>
            {
                entity.Property(e => e.AccidentId)
                    .ValueGeneratedNever()
                    .HasColumnName("AccidentID");

                entity.Property(e => e.Accident1)
                    .HasMaxLength(100)
                    .HasColumnName("Accident");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.EmpId).HasColumnName("EmpID");

                entity.Property(e => e.TransferHospital).HasMaxLength(100);

                entity.Property(e => e.Treatment).HasMaxLength(100);

                entity.Property(e => e.VolumnCode).HasMaxLength(50);

                entity.HasOne(d => d.Emp)
                    .WithMany(p => p.Accidents)
                    .HasForeignKey(d => d.EmpId)
                    .HasConstraintName("FK__Accidents__EmpID__398D8EEE");
            });

            modelBuilder.Entity<Drug>(entity =>
            {
                entity.Property(e => e.DrugId)
                    .ValueGeneratedNever()
                    .HasColumnName("DrugID");

                entity.Property(e => e.CodeDrug).HasMaxLength(50);

                entity.Property(e => e.Content).HasMaxLength(100);

                entity.Property(e => e.Guide).HasMaxLength(200);

                entity.Property(e => e.NameDrug).HasMaxLength(100);

                entity.Property(e => e.Unit).HasMaxLength(20);
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmpId)
                    .HasName("PK__Employee__AF2DBA797ABC1ADA");

                entity.Property(e => e.EmpId)
                    .ValueGeneratedNever()
                    .HasColumnName("EmpID");

                entity.Property(e => e.Birth).HasColumnType("date");

                entity.Property(e => e.DepName).HasMaxLength(50);

                entity.Property(e => e.FullName).HasMaxLength(100);

                entity.Property(e => e.Genre).HasMaxLength(10);

                entity.Property(e => e.Job).HasMaxLength(50);
            });

            modelBuilder.Entity<Injury>(entity =>
            {
                entity.Property(e => e.InjuryId)
                    .ValueGeneratedNever()
                    .HasColumnName("InjuryID");

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.EmpId).HasColumnName("EmpID");

                entity.Property(e => e.NameInjury).HasMaxLength(100);

                entity.HasOne(d => d.Emp)
                    .WithMany(p => p.Injuries)
                    .HasForeignKey(d => d.EmpId)
                    .HasConstraintName("FK__Injuries__EmpID__3C69FB99");
            });

            modelBuilder.Entity<Prescription>(entity =>
            {
                entity.Property(e => e.PrescriptionId)
                    .ValueGeneratedNever()
                    .HasColumnName("PrescriptionID");

                entity.Property(e => e.Doctor).HasMaxLength(100);

                entity.Property(e => e.DrugId).HasColumnName("DrugID");

                entity.Property(e => e.EmpId).HasColumnName("EmpID");

                entity.HasOne(d => d.Drug)
                    .WithMany(p => p.Prescriptions)
                    .HasForeignKey(d => d.DrugId)
                    .HasConstraintName("FK__Prescript__DrugI__4222D4EF");

                entity.HasOne(d => d.Emp)
                    .WithMany(p => p.Prescriptions)
                    .HasForeignKey(d => d.EmpId)
                    .HasConstraintName("FK__Prescript__EmpID__412EB0B6");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

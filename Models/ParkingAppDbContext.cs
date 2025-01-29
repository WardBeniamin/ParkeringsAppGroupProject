using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ParkeringsApp.Models;

public partial class ParkingAppDbContext : DbContext
{
    public ParkingAppDbContext()
    {
    }

    public ParkingAppDbContext(DbContextOptions<ParkingAppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Car> Cars { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Receipt> Receipts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Zone> Zones { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
            => optionsBuilder.UseSqlServer("Server=(local)\\SQLEXPRESS;Database=ParkingAppDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(e => e.CarId).HasName("PK__Cars__68A0342EA5B51335");

            entity.HasIndex(e => e.PlateNumber, "UQ__Cars__036926245901A45C").IsUnique();

            entity.Property(e => e.Model)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PlateNumber)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.Cars)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cars__UserId__412EB0B6");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__PaymentM__9B556A385C65D277");

            entity.Property(e => e.PaymentType)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Receipt>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Receipts__55433A6B9287417C");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");

            entity.HasOne(d => d.Car).WithMany(p => p.Receipts)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Receipts__CarId__47DBAE45");

            entity.HasOne(d => d.Payment).WithMany(p => p.Receipts)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Receipts__Paymen__48CFD27E");

            entity.HasOne(d => d.User).WithMany(p => p.Receipts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Receipts__UserId__45F365D3");

            entity.HasOne(d => d.Zone).WithMany(p => p.Receipts)
                .HasForeignKey(d => d.ZoneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Receipts__ZoneId__46E78A0C");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C5EB35911");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534F81097B8").IsUnique();

            entity.Property(e => e.Adress)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EncryptedPassword)
                .HasMaxLength(512)
                .IsUnicode(false);
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasMany(d => d.Payments).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserPaymentMethod",
                    r => r.HasOne<PaymentMethod>().WithMany()
                        .HasForeignKey("PaymentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UserPayme__Payme__3D5E1FD2"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UserPayme__UserI__3C69FB99"),
                    j =>
                    {
                        j.HasKey("UserId", "PaymentId").HasName("PK__UserPaym__8E3D9AEF53BDFA12");
                        j.ToTable("UserPaymentMethods");
                    });
        });

        modelBuilder.Entity<Zone>(entity =>
        {
            entity.HasKey(e => e.ZoneId).HasName("PK__Zones__601667B5E236CD87");

            entity.Property(e => e.Adress)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Fee).HasColumnType("decimal(10, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
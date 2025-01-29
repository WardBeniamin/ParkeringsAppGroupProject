﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ParkeringsApp.Models;

#nullable disable

namespace ParkeringsApp.Migrations
{
    [DbContext(typeof(ParkingAppDbContext))]
    [Migration("20250129105713_AddActiveParkingTable")]
    partial class AddActiveParkingTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ParkeringsApp.Models.ActiveParking", b =>
                {
                    b.Property<int>("ActiveParkingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ActiveParkingId"));

                    b.Property<int>("CarId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("ZoneId")
                        .HasColumnType("int");

                    b.HasKey("ActiveParkingId");

                    b.HasIndex("CarId");

                    b.HasIndex("UserId");

                    b.HasIndex("ZoneId");

                    b.ToTable("ActiveParking");
                });

            modelBuilder.Entity("ParkeringsApp.Models.Car", b =>
                {
                    b.Property<int>("CarId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CarId"));

                    b.Property<string>("Model")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("PlateNumber")
                        .IsRequired()
                        .HasMaxLength(15)
                        .IsUnicode(false)
                        .HasColumnType("varchar(15)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("CarId")
                        .HasName("PK__Cars__68A0342EA5B51335");

                    b.HasIndex("UserId");

                    b.HasIndex(new[] { "PlateNumber" }, "UQ__Cars__036926245901A45C")
                        .IsUnique();

                    b.ToTable("Cars");
                });

            modelBuilder.Entity("ParkeringsApp.Models.PaymentMethod", b =>
                {
                    b.Property<int>("PaymentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PaymentId"));

                    b.Property<string>("PaymentType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.HasKey("PaymentId")
                        .HasName("PK__PaymentM__9B556A385C65D277");

                    b.ToTable("PaymentMethods");
                });

            modelBuilder.Entity("ParkeringsApp.Models.Receipt", b =>
                {
                    b.Property<int>("TransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TransactionId"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int>("CarId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("EndTime")
                        .HasColumnType("datetime");

                    b.Property<int>("PaymentId")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("ZoneId")
                        .HasColumnType("int");

                    b.HasKey("TransactionId")
                        .HasName("PK__Receipts__55433A6B9287417C");

                    b.HasIndex("CarId");

                    b.HasIndex("PaymentId");

                    b.HasIndex("UserId");

                    b.HasIndex("ZoneId");

                    b.ToTable("Receipts");
                });

            modelBuilder.Entity("ParkeringsApp.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("Adress")
                        .HasMaxLength(250)
                        .IsUnicode(false)
                        .HasColumnType("varchar(250)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("EncryptedPassword")
                        .HasMaxLength(512)
                        .IsUnicode(false)
                        .HasColumnType("varchar(512)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Password")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.HasKey("UserId")
                        .HasName("PK__Users__1788CC4C5EB35911");

                    b.HasIndex(new[] { "Email" }, "UQ__Users__A9D10534F81097B8")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ParkeringsApp.Models.Zone", b =>
                {
                    b.Property<int>("ZoneId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ZoneId"));

                    b.Property<string>("Adress")
                        .HasMaxLength(250)
                        .IsUnicode(false)
                        .HasColumnType("varchar(250)");

                    b.Property<decimal>("Fee")
                        .HasColumnType("decimal(10, 2)");

                    b.HasKey("ZoneId")
                        .HasName("PK__Zones__601667B5E236CD87");

                    b.ToTable("Zones");
                });

            modelBuilder.Entity("UserPaymentMethod", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("PaymentId")
                        .HasColumnType("int");

                    b.HasKey("UserId", "PaymentId")
                        .HasName("PK__UserPaym__8E3D9AEF53BDFA12");

                    b.HasIndex("PaymentId");

                    b.ToTable("UserPaymentMethods", (string)null);
                });

            modelBuilder.Entity("ParkeringsApp.Models.ActiveParking", b =>
                {
                    b.HasOne("ParkeringsApp.Models.Car", "Car")
                        .WithMany()
                        .HasForeignKey("CarId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ParkeringsApp.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ParkeringsApp.Models.Zone", "Zone")
                        .WithMany("ActiveParking")
                        .HasForeignKey("ZoneId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Car");

                    b.Navigation("User");

                    b.Navigation("Zone");
                });

            modelBuilder.Entity("ParkeringsApp.Models.Car", b =>
                {
                    b.HasOne("ParkeringsApp.Models.User", "User")
                        .WithMany("Cars")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK__Cars__UserId__412EB0B6");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ParkeringsApp.Models.Receipt", b =>
                {
                    b.HasOne("ParkeringsApp.Models.Car", "Car")
                        .WithMany("Receipts")
                        .HasForeignKey("CarId")
                        .IsRequired()
                        .HasConstraintName("FK__Receipts__CarId__47DBAE45");

                    b.HasOne("ParkeringsApp.Models.PaymentMethod", "Payment")
                        .WithMany("Receipts")
                        .HasForeignKey("PaymentId")
                        .IsRequired()
                        .HasConstraintName("FK__Receipts__Paymen__48CFD27E");

                    b.HasOne("ParkeringsApp.Models.User", "User")
                        .WithMany("Receipts")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK__Receipts__UserId__45F365D3");

                    b.HasOne("ParkeringsApp.Models.Zone", "Zone")
                        .WithMany("Receipts")
                        .HasForeignKey("ZoneId")
                        .IsRequired()
                        .HasConstraintName("FK__Receipts__ZoneId__46E78A0C");

                    b.Navigation("Car");

                    b.Navigation("Payment");

                    b.Navigation("User");

                    b.Navigation("Zone");
                });

            modelBuilder.Entity("UserPaymentMethod", b =>
                {
                    b.HasOne("ParkeringsApp.Models.PaymentMethod", null)
                        .WithMany()
                        .HasForeignKey("PaymentId")
                        .IsRequired()
                        .HasConstraintName("FK__UserPayme__Payme__3D5E1FD2");

                    b.HasOne("ParkeringsApp.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK__UserPayme__UserI__3C69FB99");
                });

            modelBuilder.Entity("ParkeringsApp.Models.Car", b =>
                {
                    b.Navigation("Receipts");
                });

            modelBuilder.Entity("ParkeringsApp.Models.PaymentMethod", b =>
                {
                    b.Navigation("Receipts");
                });

            modelBuilder.Entity("ParkeringsApp.Models.User", b =>
                {
                    b.Navigation("Cars");

                    b.Navigation("Receipts");
                });

            modelBuilder.Entity("ParkeringsApp.Models.Zone", b =>
                {
                    b.Navigation("ActiveParking");

                    b.Navigation("Receipts");
                });
#pragma warning restore 612, 618
        }
    }
}

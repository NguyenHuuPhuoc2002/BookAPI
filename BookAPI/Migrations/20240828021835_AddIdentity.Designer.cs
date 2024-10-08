﻿// <auto-generated />
using System;
using BookAPI.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BookAPI.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20240828021835_AddIdentity")]
    partial class AddIdentity
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BookAPI.Data.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Gender")
                        .HasColumnType("int");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("BookAPI.Data.ChiTietHoaDon", b =>
                {
                    b.Property<Guid>("MaCT")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("DonGia")
                        .HasColumnType("float");

                    b.Property<double>("GiamGia")
                        .HasColumnType("float");

                    b.Property<Guid>("MaHD")
                        .HasMaxLength(50)
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("MaSach")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("SoLuong")
                        .HasColumnType("int");

                    b.HasKey("MaCT");

                    b.HasIndex("MaHD");

                    b.HasIndex("MaSach");

                    b.ToTable("ChiTietHoaDon");
                });

            modelBuilder.Entity("BookAPI.Data.GioHang", b =>
                {
                    b.Property<int>("GioHangId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GioHangId"));

                    b.Property<string>("MaKH")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("GioHangId");

                    b.HasIndex("MaKH")
                        .IsUnique()
                        .HasFilter("[MaKH] IS NOT NULL");

                    b.ToTable("GioHang", (string)null);
                });

            modelBuilder.Entity("BookAPI.Data.GioHangChiTiet", b =>
                {
                    b.Property<int>("GioHangChiTietId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GioHangChiTietId"));

                    b.Property<string>("Anh")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<double>("DonGia")
                        .HasColumnType("float");

                    b.Property<double>("GiamGia")
                        .HasColumnType("float");

                    b.Property<int>("GioHangId")
                        .HasColumnType("int");

                    b.Property<string>("MaSach")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("SoLuong")
                        .HasColumnType("int");

                    b.Property<string>("TenSach")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<double>("ThanhTien")
                        .HasColumnType("float");

                    b.HasKey("GioHangChiTietId");

                    b.HasIndex("GioHangId");

                    b.HasIndex("MaSach");

                    b.ToTable("ChiTietGioHang");
                });

            modelBuilder.Entity("BookAPI.Data.HoaDon", b =>
                {
                    b.Property<Guid>("MaHD")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CachThanhToan")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("CachVanChuyen")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("DiaChi")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("DienThoai")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("GhiChu")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("HoTen")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("MaKH")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("MaNV")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MaTrangThai")
                        .HasColumnType("int");

                    b.Property<DateTime?>("NgayDat")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("NgayGiao")
                        .HasColumnType("datetime2");

                    b.Property<double?>("PhiVanChuyen")
                        .HasColumnType("float");

                    b.Property<double>("TongTien")
                        .HasColumnType("float");

                    b.HasKey("MaHD");

                    b.HasIndex("MaTrangThai");

                    b.ToTable("HoaDon");
                });

            modelBuilder.Entity("BookAPI.Data.Loai", b =>
                {
                    b.Property<string>("MaLoai")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("TenLoai")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("MaLoai");

                    b.ToTable("Loai");
                });

            modelBuilder.Entity("BookAPI.Data.NhaCungCap", b =>
                {
                    b.Property<string>("MaNCC")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("DiaChi")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("DienThoai")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Email")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Logo")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("MoTa")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("NguoiLienLac")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("TenCongTy")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("MaNCC");

                    b.ToTable("NhaCungCap");
                });

            modelBuilder.Entity("BookAPI.Data.NhaXuatBan", b =>
                {
                    b.Property<int>("MaNXB")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MaNXB"));

                    b.Property<string>("DiaChi")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("DienThoai")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Email")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Logo")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("MoTa")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("NguoiLienLac")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("TenNhaXuatBan")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("MaNXB");

                    b.ToTable("NhaXuatBan");
                });

            modelBuilder.Entity("BookAPI.Data.RefreshToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ExpiredAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsRevoked")
                        .HasColumnType("bit");

                    b.Property<bool>("IsUsed")
                        .HasColumnType("bit");

                    b.Property<DateTime>("IssuedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("JwtId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Token")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("RefreshToken");
                });

            modelBuilder.Entity("BookAPI.Data.Sach", b =>
                {
                    b.Property<string>("MaSach")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Anh")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<double?>("Gia")
                        .HasColumnType("float");

                    b.Property<string>("MaLoai")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("MaNCC")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("MaNXB")
                        .HasColumnType("int");

                    b.Property<string>("MoTa")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<DateTime?>("NgayNhap")
                        .HasColumnType("datetime2");

                    b.Property<int?>("SoLuong")
                        .HasColumnType("int");

                    b.Property<int?>("SoLuongTon")
                        .HasColumnType("int");

                    b.Property<int?>("SoTap")
                        .HasColumnType("int");

                    b.Property<string>("TacGia")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("TenSach")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("MaSach");

                    b.HasIndex("MaLoai");

                    b.HasIndex("MaNCC");

                    b.HasIndex("MaNXB");

                    b.ToTable("Sach");
                });

            modelBuilder.Entity("BookAPI.Data.TrangThai", b =>
                {
                    b.Property<int>("MaTrangThai")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MaTrangThai"));

                    b.Property<string>("TenTrangThai")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("MaTrangThai");

                    b.ToTable("TrangThai");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("BookAPI.Data.ChiTietHoaDon", b =>
                {
                    b.HasOne("BookAPI.Data.HoaDon", "HoaDon")
                        .WithMany("chiTietHoaDons")
                        .HasForeignKey("MaHD")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookAPI.Data.Sach", "Sach")
                        .WithMany("chiTietHoaDons")
                        .HasForeignKey("MaSach");

                    b.Navigation("HoaDon");

                    b.Navigation("Sach");
                });

            modelBuilder.Entity("BookAPI.Data.GioHangChiTiet", b =>
                {
                    b.HasOne("BookAPI.Data.GioHang", "GioHang")
                        .WithMany("gioHangChiTiets")
                        .HasForeignKey("GioHangId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookAPI.Data.Sach", "Sach")
                        .WithMany("gioHangChiTiets")
                        .HasForeignKey("MaSach")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("GioHang");

                    b.Navigation("Sach");
                });

            modelBuilder.Entity("BookAPI.Data.HoaDon", b =>
                {
                    b.HasOne("BookAPI.Data.TrangThai", "TrangThai")
                        .WithMany("hoaDons")
                        .HasForeignKey("MaTrangThai")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TrangThai");
                });

            modelBuilder.Entity("BookAPI.Data.RefreshToken", b =>
                {
                    b.HasOne("BookAPI.Data.ApplicationUser", "User")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("User");
                });

            modelBuilder.Entity("BookAPI.Data.Sach", b =>
                {
                    b.HasOne("BookAPI.Data.Loai", "Loai")
                        .WithMany("sachs")
                        .HasForeignKey("MaLoai")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("BookAPI.Data.NhaCungCap", "NhaCungCap")
                        .WithMany("sachs")
                        .HasForeignKey("MaNCC")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("BookAPI.Data.NhaXuatBan", "NhaXuatBan")
                        .WithMany("sachs")
                        .HasForeignKey("MaNXB")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Loai");

                    b.Navigation("NhaCungCap");

                    b.Navigation("NhaXuatBan");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("BookAPI.Data.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("BookAPI.Data.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookAPI.Data.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("BookAPI.Data.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BookAPI.Data.ApplicationUser", b =>
                {
                    b.Navigation("RefreshTokens");
                });

            modelBuilder.Entity("BookAPI.Data.GioHang", b =>
                {
                    b.Navigation("gioHangChiTiets");
                });

            modelBuilder.Entity("BookAPI.Data.HoaDon", b =>
                {
                    b.Navigation("chiTietHoaDons");
                });

            modelBuilder.Entity("BookAPI.Data.Loai", b =>
                {
                    b.Navigation("sachs");
                });

            modelBuilder.Entity("BookAPI.Data.NhaCungCap", b =>
                {
                    b.Navigation("sachs");
                });

            modelBuilder.Entity("BookAPI.Data.NhaXuatBan", b =>
                {
                    b.Navigation("sachs");
                });

            modelBuilder.Entity("BookAPI.Data.Sach", b =>
                {
                    b.Navigation("chiTietHoaDons");

                    b.Navigation("gioHangChiTiets");
                });

            modelBuilder.Entity("BookAPI.Data.TrangThai", b =>
                {
                    b.Navigation("hoaDons");
                });
#pragma warning restore 612, 618
        }
    }
}

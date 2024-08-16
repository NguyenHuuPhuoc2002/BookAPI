﻿// <auto-generated />
using System;
using BookAPI.Repositories.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BookAPI.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BookAPI.Data.ChiTietHoaDon", b =>
                {
                    b.Property<int>("MaCT")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MaCT"));

                    b.Property<double>("DonGia")
                        .HasColumnType("float");

                    b.Property<double>("GiamGia")
                        .HasColumnType("float");

                    b.Property<int>("MaHD")
                        .HasMaxLength(50)
                        .HasColumnType("int");

                    b.Property<string>("MaSach")
                        .IsRequired()
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
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("GioHangId");

                    b.HasIndex("MaKH")
                        .IsUnique();

                    b.ToTable("GioHang", (string)null);
                });

            modelBuilder.Entity("BookAPI.Data.GioHangChiTiet", b =>
                {
                    b.Property<int>("GioHangChiTietId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GioHangChiTietId"));

                    b.Property<string>("Anh")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<double>("DonGia")
                        .HasColumnType("float");

                    b.Property<double>("GiamGia")
                        .HasColumnType("float");

                    b.Property<int>("GioHangId")
                        .HasColumnType("int");

                    b.Property<string>("MaSach")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("SoLuong")
                        .HasColumnType("int");

                    b.Property<string>("TenSach")
                        .IsRequired()
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
                    b.Property<int>("MaHD")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MaHD"));

                    b.Property<string>("CachThanhToan")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("CachVanChuyen")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("DiaChi")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("DienThoai")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("GhiChu")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("HoTen")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("MaKH")
                        .IsRequired()
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

                    b.Property<string>("PhiVanChuyen")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("TongTien")
                        .HasColumnType("float");

                    b.HasKey("MaHD");

                    b.HasIndex("MaKH");

                    b.HasIndex("MaTrangThai");

                    b.ToTable("HoaDon");
                });

            modelBuilder.Entity("BookAPI.Data.KhachHang", b =>
                {
                    b.Property<string>("MaKH")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("DiaChi")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("DienThoai")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("GioiTinh")
                        .HasColumnType("int");

                    b.Property<bool>("HieuLuc")
                        .HasColumnType("bit");

                    b.Property<string>("Hinh")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("HoTen")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("MatKhau")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("NgaySinh")
                        .HasColumnType("datetime2");

                    b.Property<string>("RandomKey")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("VaiTro")
                        .HasColumnType("int");

                    b.HasKey("MaKH");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("KhachHang", (string)null);
                });

            modelBuilder.Entity("BookAPI.Data.Loai", b =>
                {
                    b.Property<string>("MaLoai")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("TenLoai")
                        .IsRequired()
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
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("MaNCC");

                    b.ToTable("NhaCungCap");
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
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("MaSach");

                    b.HasIndex("MaLoai");

                    b.HasIndex("MaNCC");

                    b.ToTable("Sach");
                });

            modelBuilder.Entity("BookAPI.Data.TrangThai", b =>
                {
                    b.Property<int>("MaTrangThai")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MaTrangThai"));

                    b.Property<string>("TenTrangThai")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("MaTrangThai");

                    b.ToTable("TrangThai");
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
                        .HasForeignKey("MaSach")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("HoaDon");

                    b.Navigation("Sach");
                });

            modelBuilder.Entity("BookAPI.Data.GioHang", b =>
                {
                    b.HasOne("BookAPI.Data.KhachHang", "KhachHang")
                        .WithOne("GioHang")
                        .HasForeignKey("BookAPI.Data.GioHang", "MaKH")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("KhachHang");
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
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GioHang");

                    b.Navigation("Sach");
                });

            modelBuilder.Entity("BookAPI.Data.HoaDon", b =>
                {
                    b.HasOne("BookAPI.Data.KhachHang", "KhachHang")
                        .WithMany("hoaDons")
                        .HasForeignKey("MaKH")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookAPI.Data.TrangThai", "TrangThai")
                        .WithMany("hoaDons")
                        .HasForeignKey("MaTrangThai")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("KhachHang");

                    b.Navigation("TrangThai");
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

                    b.Navigation("Loai");

                    b.Navigation("NhaCungCap");
                });

            modelBuilder.Entity("BookAPI.Data.GioHang", b =>
                {
                    b.Navigation("gioHangChiTiets");
                });

            modelBuilder.Entity("BookAPI.Data.HoaDon", b =>
                {
                    b.Navigation("chiTietHoaDons");
                });

            modelBuilder.Entity("BookAPI.Data.KhachHang", b =>
                {
                    b.Navigation("GioHang");

                    b.Navigation("hoaDons");
                });

            modelBuilder.Entity("BookAPI.Data.Loai", b =>
                {
                    b.Navigation("sachs");
                });

            modelBuilder.Entity("BookAPI.Data.NhaCungCap", b =>
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

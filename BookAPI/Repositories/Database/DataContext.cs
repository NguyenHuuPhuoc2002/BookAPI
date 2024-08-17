using BookAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Repositories.Database
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        #region DbSet
        public DbSet<Loai> loais { get; set; }
        public DbSet<Sach> sachs { get; set; }
        public DbSet<NhaCungCap> nhaCungCaps { get; set; }
        public DbSet<HoaDon> hoaDons { get; set; }
        public DbSet<ChiTietHoaDon> chiTietHoaDons { get; set; }
        public DbSet<KhachHang> khachHangs { get; set; }
        public DbSet<TrangThai> trangThais { get; set; }
        public DbSet<GioHang> gioHangs { get; set; }
        public DbSet<NhaXuatBan> nhaXuatBans { get; set; }
        public DbSet<GioHangChiTiet> gioHangChiTiets { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<KhachHang>(entity =>
            {
                entity.ToTable(nameof(KhachHang));
                entity.HasKey(kh => kh.MaKH);
                entity.Property(kh => kh.MaKH).HasMaxLength(50);
                entity.Property(kh => kh.HoTen).HasMaxLength(50);
                entity.Property(kh => kh.DiaChi).HasMaxLength(100);
                entity.Property(kh => kh.DienThoai).HasMaxLength(20);
                entity.Property(kh => kh.Hinh).HasMaxLength(100);
                entity.Property(kh => kh.RandomKey).HasMaxLength(50);
                entity.Property(kh => kh.Email).HasMaxLength(50);
                entity.HasIndex(kh => kh.Email).IsUnique();
            });
            modelBuilder.Entity<GioHang>(entity =>
            {
                entity.ToTable("GioHang");
                entity.HasKey(gh => gh.GioHangId);
                entity.HasIndex(gh => gh.MaKH).IsUnique();
                entity.HasMany(gh => gh.gioHangChiTiets)
                 .WithOne(ghct => ghct.GioHang)
                 .HasForeignKey(ghct => ghct.GioHangId)
                 .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(gh => gh.KhachHang)
                       .WithOne(kh => kh.GioHang)
                       .HasForeignKey<GioHang>(gh => gh.MaKH)
                       .OnDelete(DeleteBehavior.Cascade);
                   
            });

            modelBuilder.Entity<GioHangChiTiet>(entity =>
            {
                entity.HasOne(ghct => ghct.Sach)
                      .WithMany(s => s.gioHangChiTiets) 
                      .HasForeignKey(ghct => ghct.MaSach) 
                      .OnDelete(DeleteBehavior.Cascade); 
            });


            modelBuilder.Entity<Sach>(entity =>
            {
                entity.HasMany(s => s.gioHangChiTiets)
                .WithOne(ghct => ghct.Sach)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(s => s.Loai)
                          .WithMany(l => l.sachs)
                          .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(s => s.NhaCungCap)
                      .WithMany(ncc => ncc.sachs)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(s => s.NhaXuatBan)
                      .WithMany(ncc => ncc.sachs)
                      .OnDelete(DeleteBehavior.SetNull);
            });

        }
    }
}

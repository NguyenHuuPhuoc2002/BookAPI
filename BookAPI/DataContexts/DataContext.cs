using BookAPI.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Database
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        #region DbSet
        public DbSet<Loai> Loais { get; set; }
        public DbSet<Sach> Sachs { get; set; }
        public DbSet<NhaCungCap> NhaCungCaps { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }
       /* public DbSet<KhachHang> KhachHangs { get; set; }*/
        public DbSet<TrangThai> TrangThais { get; set; }
        public DbSet<GioHang> GioHangs { get; set; }
        public DbSet<NhaXuatBan> NhaXuatBans { get; set; }
        public DbSet<GioHangChiTiet> GioHangChiTiets { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           /* modelBuilder.Entity<KhachHang>(entity =>
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
            });*/
            modelBuilder.Entity<GioHang>(entity =>
            {
                entity.ToTable("GioHang");
                entity.HasKey(gh => gh.GioHangId);
                entity.HasIndex(gh => gh.MaKH).IsUnique();
                entity.HasMany(gh => gh.gioHangChiTiets)
                 .WithOne(ghct => ghct.GioHang)
                 .HasForeignKey(ghct => ghct.GioHangId)
                 .OnDelete(DeleteBehavior.Cascade);

                /*entity.HasOne(gh => gh.KhachHang)
                       .WithOne(kh => kh.GioHang)
                       .HasForeignKey<GioHang>(gh => gh.MaKH)
                       .OnDelete(DeleteBehavior.Cascade);*/



            });

            modelBuilder.Entity<GioHangChiTiet>(entity =>
            {
                entity.HasOne(ghct => ghct.Sach)
                      .WithMany(s => s.gioHangChiTiets)
                      .HasForeignKey(ghct => ghct.MaSach)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RefreshToken>()
                         .HasKey(rt => rt.Id);
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

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

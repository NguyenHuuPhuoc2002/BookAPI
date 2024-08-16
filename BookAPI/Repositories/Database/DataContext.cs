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

        }
    }
}

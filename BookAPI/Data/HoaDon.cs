using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookAPI.Data
{
    [Table("HoaDon")]
    public class HoaDon
    {
        [Key]
        [StringLength(50)]
        public string MaHD { get; set; }

        [StringLength(50)]
        public string MaKH { get; set; }
        public DateTime? NgayDat { get; set; }
        public DateTime? NgayGiao { get; set; }
        public int MaTrangThai { get; set; }

        [StringLength(50)]
        public string HoTen { get; set; }

        [StringLength(50)]
        public string DiaChi { get; set; }

        [StringLength(20)]
        public string DienThoai { get; set; }

        [StringLength(50)]
        public string CachThanhToan { get; set; }

        [StringLength(50)]
        public string CachVanChuyen { get; set; }
        public string PhiVanChuyen { get; set; }
        public string? MaNV { get; set; }

        [StringLength(200)]
        public string? GhiChu { get; set; }
        public double TongTien { get; set; }

        [ForeignKey("MaKH")]
        public KhachHang KhachHang { get; set; }

        [ForeignKey("MaTrangThai")]
        public TrangThai TrangThai { get; set; }
        public ICollection<ChiTietHoaDon> chiTietHoaDons { get; set; }

    }
}

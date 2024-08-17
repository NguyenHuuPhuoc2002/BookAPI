using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookAPI.Data
{
    [Table("Sach")]
    public class Sach
    {
        [Key]
        [StringLength(50)]
        public string MaSach {  get; set; } 

        [StringLength(50)]
        public string TenSach {  get; set; }

        public int? SoLuong {  get; set; }

        public double? Gia{  get; set; }

        public int? SoTap{  get; set; }
 
        [StringLength (50)]
        public string? Anh{  get; set; }

        public DateTime? NgayNhap{  get; set; }

        [StringLength(50)]
        public string? TacGia{  get; set; }

        [StringLength(50)]
        public string? MaLoai{  get; set; }
        public int? SoLuongTon { get; set; }

        [StringLength(250)]
        public string? MoTa { get; set; }

        [StringLength(50)]
        public string? MaNCC { get; set; }
        public int? MaNXB { get; set; }

        [ForeignKey("MaLoai")]
        public Loai Loai { get; set; }

        [ForeignKey("MaNXB")]
        public NhaXuatBan NhaXuatBan { get; set; }

        [ForeignKey("MaNCC")]
        public NhaCungCap NhaCungCap { get; set; }

        public ICollection<ChiTietHoaDon> chiTietHoaDons { get; set; }
        public ICollection<GioHangChiTiet> gioHangChiTiets { get; set; }

        
    }
}

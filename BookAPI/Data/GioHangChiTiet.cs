using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookAPI.Data
{
    [Table("ChiTietGioHang")]
    public class GioHangChiTiet
    {
        [Key]
        public int GioHangChiTietId { get; set; }

        [StringLength(100)]
        public string Anh { get; set; }

        [StringLength(50)]
        public string TenSach { get; set; }

        public double DonGia { get; set; }
        public int SoLuong { get; set; }
        public double ThanhTien { get; set; }
        public double GiamGia { get; set; }

        public int GioHangId { get; set; }  

        [StringLength(50)]
        public string MaSach { get; set; } 

        [ForeignKey("MaSach")]
        public Sach Sach { get; set; }  

        [ForeignKey("GioHangId")]
        public GioHang GioHang { get; set; } 
    }
}

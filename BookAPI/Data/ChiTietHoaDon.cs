using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookAPI.Data
{
    [Table("ChiTietHoaDon")]
    public class ChiTietHoaDon
    {
        [Key]
        public int MaCT { get; set; }
        [StringLength(50)]
        public string MaHD{ get; set; }

        [StringLength(50)]
        public string MaSach { get; set; }

        public double DonGia { get; set; }

        public int SoLuong { get; set; }

        public double GiamGia { get; set; }

        [ForeignKey("MaHD")]
        public HoaDon HoaDon { get; set; }

        [ForeignKey("MaSach")]
        public Sach Sach { get; set; }
    }
}

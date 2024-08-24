using System.ComponentModel.DataAnnotations;

namespace BookAPI.Models
{
    public class ChiTietHDModel
    {
        public Guid MaCT { get; set; }
        public Guid MaHD { get; set; }
        public string MaSach { get; set; }

        public double DonGia { get; set; }

        public int SoLuong { get; set; }

        public double GiamGia { get; set; }
        public string? Anh{ get; set; }
    }
}

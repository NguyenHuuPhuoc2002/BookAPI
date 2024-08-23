using System.ComponentModel.DataAnnotations;

namespace BookAPI.Models
{
    public class HoaDonModel
    {
        public Guid MaHD { get; set; }
        public string MaKH { get; set; }
        [DataType(DataType.Date)]
        public DateTime? NgayDat { get; set; }
        [DataType(DataType.Date)]
        public DateTime? NgayGiao { get; set; }
        public string? TrangThai { get; set; }
        public string HoTen { get; set; }
        public string DiaChi { get; set; }
        public string DienThoai { get; set; }
        public string? CachThanhToan { get; set; }
        public string? CachVanChuyen { get; set; }
        public double PhiVanChuyen { get; set; }
        public string? MaNV { get; set; }
        public int MaTrangThai { get; set; }
        public string? GhiChu{ get; set; }

    }
}

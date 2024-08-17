using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookAPI.Data
{
    [Table("NhaXuatBan")]
    public class NhaXuatBan
    {
        [Key]
        [StringLength(50)]
        public int MaNXB { get; set; }

        [StringLength(50)]
        public string TenNhaXuatBan { get; set; }

        [StringLength(100)]
        public string? Logo { get; set; }

        [StringLength(50)]
        public string? NguoiLienLac { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(20)]
        public string DienThoai { get; set; }

        [StringLength(50)]
        public string? DiaChi { get; set; }
        [StringLength(200)]
        public string? MoTa { get; set; }

        public ICollection<Sach> sachs {  get; set; }
    }
}

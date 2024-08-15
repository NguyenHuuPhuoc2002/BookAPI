using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookAPI.Data
{
    [Table("TrangThai")]
    public class TrangThai
    {
        [Key]
        public int MaTrangThai { get; set; }

        [StringLength(50)]
        public string TenTrangThai { get; set;}

        public ICollection<HoaDon> hoaDons { get; set; }
    }
}

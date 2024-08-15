using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookAPI.Data
{
    [Table("Loai")]
    public class Loai
    {
        [Key]
        [StringLength(50)]
        public string MaLoai {  get; set; }

        [StringLength(50)]
        public string TenLoai {  get; set; } 

        public ICollection<Sach> sachs { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace BookAPI.Models
{
    public class LoaiModel
    {
        [Required]
        public string MaLoai { get; set; }
        [Required]
        public string TenLoai { get; set; }
    }
}

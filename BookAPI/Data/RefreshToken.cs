using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookAPI.Data
{
    [Table("RefreshToken")]
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string MaKH { get; set; }
        [ForeignKey(nameof(MaKH))]
        public KhachHang nguoiDung { get; set; }
        public string Token { get; set; }
        public string JwtId { get; set; }
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime ExpiredAt { get; set; }
    }
}

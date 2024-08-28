using BookAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookAPI.Data
{
    public class GioHang
    {
        public int GioHangId {  get; set; }
        public string MaKH {  get; set; }

        /*[ForeignKey("MaKH")]
        public KhachHang KhachHang { get; set; }*/
        public ICollection<GioHangChiTiet> gioHangChiTiets { get; set; }

    }
}

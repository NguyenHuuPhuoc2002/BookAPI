using BookAPI.Helper;
using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonHangController : ControllerBase
    {
        private readonly IHoaDonRepository _hoaDon;

        public DonHangController(IHoaDonRepository hoaDon) 
        {
            _hoaDon = hoaDon;
        }

        [HttpPost("orders")]
        public async Task<IActionResult> GetOrders(HoaDonModel model)
        {
            var maKh = User.FindFirst(MyConstants.CLAIM_CUSTOMER_ID)?.Value;
           // await _hoaDon.AddAsync(model);
            return Ok();
        }
    }
}

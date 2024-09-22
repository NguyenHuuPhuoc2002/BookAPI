using BookAPI.Helper;
using BookAPI.Models;
using BookAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRole.ADMIN)]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _role;

        public RolesController(IRoleService role)
        {
            _role = role;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoleAll()
        {
            var result = await _role.GetRoleAllAsync();
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Lấy roles thành công",
                Data = result
            });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(string id)
        {
            var result = await _role.GetRoleByIdAsync(id);
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Lấy role thành công",
                Data = result
            });
        }
        [HttpPost]
        public async Task<IActionResult> AddRole(string roleName)
        {
            var result = await _role.AddRoleAsync(roleName);
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Thêm role thành công",
                Data = result
            });
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var result = await _role.DeleteRoleAsync(id);
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Xóa role thành công",
                Data = result
            });
        }
        [HttpPut]
        public async Task<IActionResult> UpdateRole(IdentityRole role)
        {
            if (!ModelState.IsValid)
            {
                throw new MissingFieldException("Nhập đầy đủ thông tin");
            }
            var result = await _role.UpdateRoleAsync(role);
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Cập nhật role thành công",
                Data = result
            });

        }
    }
}

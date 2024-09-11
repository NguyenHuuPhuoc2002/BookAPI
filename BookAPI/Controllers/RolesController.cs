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
            try
            {
                var result = await _role.GetRoleAllAsync();
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Lấy roles thành công",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(string id)
        {
            try
            {
                var result = await _role.GetRoleByIdAsync(id);
                if(result == null)
                {
                    return NotFound();
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Lấy role thành công",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddRole(string roleName)
        {
            try
            {
                var result = await _role.AddRoleAsync(roleName);
                if (result)
                {
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Thêm role thành công",
                        Data = result
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteRole(string id)
        {
            try
            {
                var findRole = await _role.GetRoleByIdAsync(id);
                if (findRole == null)
                {
                    return NotFound();
                }
                var result = await _role.DeleteRoleAsync(id);
                if (result)
                {
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Xóa role thành công",
                        Data = result
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateRole(IdentityRole role)
        {
            try
            {
                var findRole = await _role.GetRoleByIdAsync(role.Id);
                if (findRole == null)
                {
                    return NotFound();
                }
                var result = await _role.UpdateRoleAsync(role);
                if (result)
                {
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Cập nhật role thành công",
                        Data = result
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

    }
}

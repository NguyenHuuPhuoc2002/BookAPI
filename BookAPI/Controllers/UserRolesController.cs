using BookAPI.Helper;
using BookAPI.Models;
using BookAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRole.ADMIN)]
    public class UserRolesController : ControllerBase
    {
        private readonly IUserRoleService _userRole;
        private readonly IAccountService _account;
        private readonly IRoleService _role;

        public UserRolesController(IUserRoleService userRole, IRoleService role, IAccountService account)
        {
            _userRole = userRole;
            _account = account;
            _role = role;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(string? email)
        {
            try
            {
                var result = await _userRole.GetAllAsync(email);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Lấy thành công",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddUserRole(string email, string roleName)
        {
            try
            {
                var findUser = await _account.FindByEmailAsync(email);
                if (findUser == null)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = true,
                        Message = "User không tồn tại"
                    });
                }
                var findRole = await _role.GetRoleByNameAsync(roleName);
                if (findRole == null)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = true,
                        Message = "Role không tồn tại"
                    });
                }
                var result = await _userRole.AddRoleToUserAsync(email, roleName);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Thêm thành công",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteUserRole(string email, string roleName)
        {
            try
            {
                var findUser = await _account.FindByEmailAsync(email);
                if (findUser == null)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = true,
                        Message = "User không tồn tại"
                    });
                }
                var findRole = await _role.GetRoleByNameAsync(roleName);
                if (findRole == null)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = true,
                        Message = "Role không tồn tại"
                    });
                }
                var result = await _userRole.DeleteRoleUserAsync(email, roleName);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Xóa thành công",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}

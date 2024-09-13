using AutoMapper;
using BookAPI.Data;
using BookAPI.Helper;
using BookAPI.Models;
using BookAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRole.ADMIN)]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _user;
        private readonly IMapper _mapper;

        public UsersController(IUserService user, IMapper mapper)
        {
            _user = user;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int? page, int? pageSize)
        {
            int _page = page ?? 1;
            int _pagSize = pageSize ?? 5;
            try
            {
                var result = await _user.GetAllAsync(_page, _pagSize);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Lấy user thành công",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("search")]
        public async Task<IActionResult> Search(string key, int? page, int? pageSize)
        {
            int _page = page ?? 1;
            int _pagSize = pageSize ?? 5;
            IEnumerable<ApplicationUser> result;
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    result = await _user.GetAllAsync(_page, _pagSize);
                }
                else
                {
                    result = await _user.Search(key, _page, _pagSize);
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Tìm kiếm user thành công",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddUser(UserModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var findUser = await _user.GetByEmailAsync(model.Email);
                    if (findUser != null)
                    {
                        return BadRequest(new ApiResponse
                        {
                            Success = true,
                            Message = "Email đã tồn tại"
                        });
                    }
                   
                    var result = await _user.AddAsync(model);
                    if (result)
                    {
                        return Ok(new ApiResponse
                        {
                            Success = true,
                            Message = "Thêm user thành công",
                            Data = result
                        });
                    }
                }
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Lỗi đầu vào"
                });
                
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }  
        [HttpPut]
        public async Task<IActionResult> UpdateUser(UserModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var findUser = await _user.GetByEmailAsync(model.Email);
                    if (findUser == null)
                    {
                        return NotFound();
                    }
                    var user = new ApplicationUser
                    {
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        PhoneNumber = model.PhoneNumber,
                        UserName = model.Email,
                        Image = model.Image,
                        Gender = model.Gender,
                        DateOfBirth = model.BirthDate
                    };
                    var result = await _user.UpdateAsync(user);
                    if (result)
                    {
                        return Ok(new ApiResponse
                        {
                            Success = true,
                            Message = "Cập nhật user thành công",
                            Data = result
                        });
                    }
                }
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Lỗi đầu vào"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string email)
        {
            try
            {
                var findUser = await _user.GetByEmailAsync(email);
                if (findUser == null)
                {
                    return NotFound();
                }
                var result = await _user.DeleteAsync(email);
                if (result)
                {
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Xóa user thành công",
                        Data = result
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}

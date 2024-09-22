using AutoMapper;
using BookAPI.Data;
using BookAPI.Helper;
using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using Common.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace BookAPI.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AccountRepository> _logger;
        private readonly IUrlHelper _urlHelper;

        public AccountRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration, RoleManager<IdentityRole> roleManager, ILogger<AccountRepository> logger, IUrlHelper urlHelper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _logger = logger;
            _urlHelper = urlHelper;
        }

        public async Task<bool> ChangePasswordAsync(ApplicationUser user, ChangePasswordModel model)
        {
            try
            {
                _logger.LogInformation("Thực hiện thay đổi mật khẩu");
                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Thực hiện thay đổi mật khẩu thành công");
                    return true;
                }
                throw new AppException("Đổi mật khẩu không thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError("Xảy ra lỗi khi thực hiện thay đổi mật khẩu");
                throw;
            }
        }

        public Task<ApplicationUser> FindByEmailAsync(string email)
        {
            try
            {
                var user = _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("Không tìm thấy user {email}", email);
                    throw new KeyNotFoundException("User không tồn tại");
                }
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Xảy ra lỗi khi tìm kiếm user");
                throw;
            }
        }

        public async Task<ApplicationUser> FindByIdAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if(user == null)
                {
                    _logger.LogWarning("Không tìm thấy user {email}", id);
                    throw new KeyNotFoundException("Không tìm thấy user");
                }
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Xảy ra lỗi khi tìm kiếm user");
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetRolesAsync(ApplicationUser model)
        {
            try
            {
                _logger.LogInformation("Thực hiện truy vấn lấy các role của user {email}", model.Email);
                var useRole = await _userManager.GetRolesAsync(model);
                return useRole;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, "Xảy ra lỗi khi lấy role của user {email}", model.Email);
                throw;
            }
        }

        public async Task<IdentityUser> SignInAsync(SignInModel model)
        {
            _logger.LogInformation("Truy vấn tìm user");
            var user = await _userManager.FindByEmailAsync(model.Email);
            _logger.LogInformation("Kiểm tra thông tin đăng nhập");
            var passWordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (user == null || !passWordValid)
            {
                _logger.LogInformation("User không tồn tại");
                throw new KeyNotFoundException("User không tồn tại");
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            if (!result.Succeeded)
            {
                _logger.LogInformation("Thông tin đăng nhập không chính xác");
                throw new KeyNotFoundException("Thông tin đăng nhập không chính xác");
            }

            return user;
        }

        public async Task<IdentityResult> SignUpAsync(SignUpModel model)
        {
            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Gender = model.Gender,
                DateOfBirth = model.DayOfBirth,
                Image = model.Hinh,
                UserName = model.Email
            };
            _logger.LogInformation("Tạo mới user");
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {               
                await _userManager.AddToRoleAsync(user, AppRole.CUSTOMER);
            }

            return result;
        }
        public async Task<LinkMailModel> ForgetPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var link = _urlHelper.Action("ResetPassword", new { encodedToken });
                var result = new LinkMailModel
                {
                    Email = email,
                    Link = link
                };
                return await Task.FromResult(result);

            }
            throw new KeyNotFoundException("Email is not exist");
        }
        public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser model)
        {
            try
            {
                _logger.LogInformation("Thực hiện Generate Password Reset Token");
                var token = await _userManager.GeneratePasswordResetTokenAsync(model);
                _logger.LogInformation("Thực hiện Generate Password Reset Token thành công");
                return token;
            }
            catch( Exception ex)
            {
                _logger.LogError(ex.Message, "Xảy ra lỗi khi thực hiện Generate Password Reset Token");
                throw;
            }
        }
        public async Task<bool> ResetPasswordAsync(ApplicationUser model, string token, string newPassword)
        {
            try
            {
                _logger.LogInformation("Thực hiện Reset Password cho {email}", model.UserName);
                var user = await _userManager.FindByEmailAsync(model.UserName);
                if (user == null)
                {
                    _logger.LogWarning("Không tìm thấy user: {email}", model.UserName);
                    throw new KeyNotFoundException($"Không tìm thấy user: {model.UserName}");
                }
                var resetPassword = await _userManager.ResetPasswordAsync(user, token, newPassword);
                if (!resetPassword.Succeeded)
                {
                    _logger.LogError("Reset Password cho user: {email} không thành công", model.UserName);
                    throw new AppException("Reset Password không thành công");
                }
                _logger.LogInformation("Reset Password cho user: {email} thành công", model.UserName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Xảy ra lỗi khi thực hiện Generate Password Reset Token");
                throw;
            }
        }
    }
}

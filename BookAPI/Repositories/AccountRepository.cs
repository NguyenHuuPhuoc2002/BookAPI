using AutoMapper;
using BookAPI.Data;
using BookAPI.Helper;
using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BookAPI.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AccountRepository> _logger;

        public AccountRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration, RoleManager<IdentityRole> roleManager, ILogger<AccountRepository> logger,
            IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _logger = logger;
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
                return null;
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            if (!result.Succeeded)
            {
                _logger.LogInformation("Thông tin đăng nhập chính xác");
                return null;
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
                //kiểm tra role Customer có chưa
                if (!await _roleManager.RoleExistsAsync(AppRole.CUSTOMER))
                {
                    await _roleManager.CreateAsync(new IdentityRole(AppRole.CUSTOMER));
                }
                await _userManager.AddToRoleAsync(user, AppRole.CUSTOMER);

            }

            return result;
        }
    }
}

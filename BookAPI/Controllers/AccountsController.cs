using BookAPI.Common;
using BookAPI.Data;
using BookAPI.Helper;
using BookAPI.Models;
using BookAPI.Repositories;
using BookAPI.Repositories.Interfaces;
using BookAPI.Services.Interfaces;
using Common.Exceptions;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MimeKit.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Service.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IGioHangService _cart;
        private readonly IAccountService _account;
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenService _refreshToken;
        private readonly AppSetting _appSettings;
        private readonly ILogger<AccountsController> _logger;
        private readonly IMailService _email;
        private readonly IResponseCacheService _responseCacheService;
        private readonly UserManager<ApplicationUser> _userManager;
        private SecurityToken validatedToken;

        public AccountsController(IAccountService account, IConfiguration configuration, IRefreshTokenService refreshToken,
                                   IOptionsMonitor<AppSetting> optionsMonitor, ILogger<AccountsController> logger, IGioHangService cart,
                                   IMailService mail, UserManager<ApplicationUser> userManager, IResponseCacheService responseCacheService)
        {
            _cart = cart;
            _account = account;
            _configuration = configuration;
            _refreshToken = refreshToken;
            _appSettings = optionsMonitor.CurrentValue;
            _logger = logger;
            _email = mail;
            _responseCacheService = responseCacheService;
            _userManager = userManager;
        }
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> SignOut([FromBody] LogOutRequestModel requestToken)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;          
            if (requestToken.AccessToken == TokenGlobalVariable.Token)
            {
                var dataRequest = new LogOutRequestModel
                {
                    AccessToken = requestToken.AccessToken,
                };

                var cacheDataString = JsonConvert.SerializeObject(dataRequest);
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadToken(requestToken.AccessToken) as JwtSecurityToken;
                var expires = token.ValidTo; 
                var remainingTime = expires - DateTime.UtcNow;

                await _responseCacheService.SetCacheReponseAsync(email, cacheDataString, remainingTime);
                var storedToken = await _refreshToken.GetTokenAsync(requestToken.RefreshToken);
                if (storedToken != null)
                {
                    storedToken.IsRevoked = true;
                    await _refreshToken.UpdateAsync(storedToken, requestToken.RefreshToken);
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Đăng xuất thành công"
                });
            }

            throw new Exception();
        }

        [HttpPost("login")]
        public async Task<IActionResult> SignIn(SignInModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new MissingFieldException("Nhập đầy đủ thông tin bắt buộc");
            }

            _logger.LogInformation("Yêu cầu đăng nhập từ user có email {email}", model.Email);
            var user = await _account.SignInAsync(model);
            if (user == null)
            {
                _logger.LogWarning("Không tồn tại user có email {email}", model.Email);
                return Unauthorized();
            }

            var token = await GenerateToken(user);
            var cachedToken = await _responseCacheService.GetCacheResponseAsync(token.AccessToken);
            GlobalVariables.email = user.UserName;
            TokenGlobalVariable.Token = token.AccessToken;
            _logger.LogInformation("Đăng nhập thành công");
            return Ok(token);
        }

        [HttpPost("register")]
        public async Task<IActionResult> SignUp([FromForm] SignUpModel model)
        {
            _logger.LogInformation("Yêu cầu đăng kí từ user có email {email}", model.Email);
            if (!ModelState.IsValid)
            {
                throw new MissingFieldException("Nhập đầy đủ thông tin bắt buộc");
            }
            if (model.Image != null)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "KhachHang", model.Image.FileName);
                using (var stream = System.IO.File.Create(path))
                {
                    await model.Image.CopyToAsync(stream);
                }
                model.Hinh = "/images/KhachHang/" + model.Image.FileName;
            }
            else
            {
                model.Hinh = "";
            }
            var result = await _account.SignUpAsync(model);
            _logger.LogInformation("Yêu cầu đăng kí từ user có email {email} thành công", model.Email);
            // await CreateCartAsync(model.Email);
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Đăng kí thành công",
                Data = model
            });
        }
        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            //sinh số ngẫu nhiên  
            using (var rng = RandomNumberGenerator.Create())
            {
                //lưu vào mảng ramdom
                rng.GetBytes(random);
                //chuyển mảng byte thành chuỗi Base64
                return Convert.ToBase64String(random);
            }
        }
        private async Task<TokenModel> GenerateToken(IdentityUser user)
        {
            //thanh cong thi tao ra cac quyen
            var authClaim = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            };

            var appUser = new ApplicationUser
            {
                Email = user.Email,
                PasswordHash = user.PasswordHash
            };
            var _user = await _account.FindByEmailAsync(user.Email);
            //lay usseRole
            var useRole = await _account.GetRolesAsync(_user);
            foreach (var role in useRole)
            {
                authClaim.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }

            var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secret));

            //tao moi token
            var token = new JwtSecurityToken(
                issuer: _appSettings.ValidIssuer,
                audience: _appSettings.ValidAudience,
                expires: DateTime.Now.AddMinutes(20),
                claims: authClaim,
                signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature)

                );
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var accessToken = jwtTokenHandler.WriteToken(token);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                JwtId = token.Id,
                UserId = user.Id,
                Token = refreshToken,
                IsUsed = false,
                IsRevoked = false,
                IssuedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddHours(1),
            };

            await _refreshToken.AddAsync(refreshTokenEntity);
            _logger.LogInformation("Tạo thành công AccessToken và RefreshToken");
            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };
        }
        [HttpPost("renew-access-token")]
        public async Task<IActionResult> RenewToken(TokenModel tokenModel)
        {
            _logger.LogInformation("Tạo RefreshToken");
            //check xem token gửi lên nó còn hợp lệ không trước khi cấp phát một access token mới.
            var jwtTokenHandler = new JwtSecurityTokenHandler(); //sử dụng để tạo và viết token JWT.
            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.Secret); //Chuyển đổi khóa bí mật thành mảng byte.

            //Cấu hình
            var tokenValidateParam = new TokenValidationParameters
            {
                //tự cấp token
                ValidateIssuer = false,
                ValidateAudience = false,
                //ký vào token
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
                ValidateIssuerSigningKey = true,

                ClockSkew = TimeSpan.Zero,

                ValidateLifetime = false// ko kiem tra token het hang  
            };
            try
            {
                _logger.LogDebug("Kiểm tra định dạng token");
                //check 1: AccessToken valid format
                var tokenInverification = jwtTokenHandler.ValidateToken(tokenModel.AccessToken, tokenValidateParam, out validatedToken);
                //check 2: check alg
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512Signature, StringComparison.InvariantCultureIgnoreCase);
                    if (!result)
                    {
                        _logger.LogWarning("Token không hợp lệ");
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Invalid token"
                        });
                    }
                }
                var storedToken = await _refreshToken.GetTokenAsync(tokenModel.RefreshToken);
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                // New Check: Compare userId in storedToken with current userId
                if (storedToken.UserId != userId) 
                {
                    _logger.LogWarning("Refresh token không khớp với userId hiện tại");
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Refresh token does not match current user"
                    });
                }
                _logger.LogDebug("Kiểm tra token đã hết hạn chưa");
                //check 3: Check accessToken expire?
                var utcExpireDate = long.Parse(tokenInverification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expireDate = ConvertUnixTimeToDaateTime(utcExpireDate);

                if (expireDate > DateTime.UtcNow)
                {
                    _logger.LogWarning("Token đã hết hạn");
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Access token has not yet expired"
                    });
                }

                _logger.LogDebug("Kiểm tra token đã tồn tại trong csdl hay chưa");
                //check 4: check refreshtoken exist in DB
              
                if (storedToken is null)
                {
                    _logger.LogWarning("Token đã tồn tại trong csdl");
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Refresh token doesn't exist"
                    });
                }

                _logger.LogDebug("Kiểm tra token đã được sử dụng chưa");
                //check 5: check refresh is used/ revoked ?
                if (storedToken.IsUsed)
                {
                    _logger.LogWarning("Token đã được sử dụng");
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Refresh token has been exist"
                    });
                }
                _logger.LogDebug("Kiểm tra có bị hủy không");
                if (storedToken.IsRevoked)
                {
                    _logger.LogWarning("Token đã bị hủy");
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Refresh token has been Revoked"
                    });
                }

                _logger.LogDebug("Dịch ngược để lấy JwtId từ chuỗi token và so sánh");
                //check 6: AccessToken ID = JwID in RefreshToken // dịch ngược lại để lấy JwtId từ chuỗi token
                var jti = tokenInverification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (storedToken.JwtId != jti)
                {
                    _logger.LogWarning("Token không khớp");
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Token doesn't match"
                    });
                }

                _logger.LogInformation("Cập nhật token");
                //check 7: update token is used
                storedToken.IsRevoked = true;
                storedToken.IsUsed = true;
                await _refreshToken.UpdateAsync(storedToken, tokenModel.RefreshToken);

                //create new token
                var user = await _account.FindByIdAsync(storedToken.UserId);
                var token = await GenerateToken(user);
                _logger.LogInformation("Tạo refresh token thành công");
                TokenGlobalVariable.Token = token.AccessToken;
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Renew Token Success",
                    Data = token
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Xảy ra lỗi khi tạo refresh token");
                throw;
            }

        }
        private DateTime ConvertUnixTimeToDaateTime(long utcExpireDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();
            return dateTimeInterval;
        }
        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            var mail = await _account.ForgetPassword(email);
            var request = new MailRequest();
            request.ToEmail = email;
            request.Subject = "ForgetPassword";
            request.Body = mail.Link;
            var result = await _email.SendEmail(request);
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Success"
            });
        }
        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var maKh = User.FindFirst(ClaimTypes.Email)?.Value;
            if (!ModelState.IsValid)
            {
                throw new MissingFieldException("Nhập thông tin!");
            }
            _logger.LogInformation("Yêu cầu thay đổi mật khẩu từ {email}", maKh);
            var user = await _account.FindByEmailAsync(maKh);
            var result = await _account.ChangePasswordAsync(user, model);
            _logger.LogInformation("Yêu cầu thay đổi mật khẩu từ {email} thành công", maKh);
            return NoContent();
        }

        [HttpPost("reset-password-token")]
        public async Task<IActionResult> ResetPasswordToken([FromBody] ResetPasswordTokenModel model)
        {
            var user = await _account.FindByEmailAsync(model.Email);
            var token = await _account.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            return Ok(new { encodedToken = encodedToken });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var decodedBytes = WebEncoders.Base64UrlDecode(model.Token);
            var decodedToken = Encoding.UTF8.GetString(decodedBytes);
            var user = await _account.FindByEmailAsync(model.Email);
            if (string.Compare(model.NewPassword, model.ConfirmPassword) != 0)
            {
                throw new MissingFieldException("NewPassword và ConfirmPassword không trùng khớp");
            }
            if (string.IsNullOrEmpty(model.Token))
            {
                throw new AppException("Token không hợp lệ");
            }
            var result = await _account.ResetPasswordAsync(user, decodedToken, model.NewPassword);
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Reset password thành công",
                Data = model
            });
        }
       /* private async Task<GioHang> CreateCartAsync(string maKH)
        {
            try
            {
                var createCart = new GioHang { MaKH = maKH };
                await _cart.AddAsync(createCart);
                _logger.LogInformation("Tạo giỏ hàng cho khách hàng có mã {CustomerId}", maKH);
                return createCart;
            }
            catch (Exception ex)
            {
                _logger.LogError("Xảy ra lỗi khi thêm giỏ hàng");
                throw;
            }
        }*/
    }
}

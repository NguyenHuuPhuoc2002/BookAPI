using BookAPI.Data;
using BookAPI.Helper;
using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using BookAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhachHangController : ControllerBase
    {
        private readonly IKhachHangService _khachHang;
        private readonly ILogger<KhachHangController> _logger;
        private readonly IRefreshTokenService _refreshToken;
        private readonly AppSetting _appSettings;
        private SecurityToken validatedToken;

        public KhachHangController(IKhachHangService khachHang, ILogger<KhachHangController> logger,
                                    IOptionsMonitor<AppSetting> optionsMonitor, IRefreshTokenService refreshToken)
        {
            _khachHang = khachHang;
            _logger = logger;
            _refreshToken = refreshToken;
            _appSettings = optionsMonitor.CurrentValue;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogIn(LogInModel model)
        {
            _logger.LogInformation("Thực hiện đăng nhập với tên đăng nhập {UserName} và mật khẩu {Password}", model.UserName, model.Password);
            var login = await _khachHang.CheckLogIn(model);
            if (login == null)
            {
                _logger.LogWarning("Không tìm thấy khách hàng với tên đăng nhập {UserName} và mật khẩu {Password}", model.UserName, model.Password);
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Không tìm thấy khách hàng"
                });
            }

            //Cấp token 
            var token = await GenerateToken(login);
            _logger.LogInformation("Đăng nhập thành công {UserName}", login.MaKH);
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Đăng nhập thành công",
                Data = token
            });

        }

        private async Task<TokenModel> GenerateToken(KhachHang user)
        {
            _logger.LogInformation("Bắt đầu tạo token cho khách hàng: {UserId}", user.MaKH);
            var jwtTokenHandler = new JwtSecurityTokenHandler(); //sử dụng tạo và viết token
            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);//Chuyển đổi khóa bí mật (SecretKey) thành mảng byte.

            //Mô tả cấu hình token JWT
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.HoTen),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(MyConstants.CLAIM_USER_NAME, user.MaKH),
                    new Claim(MyConstants.CLAIM_CUSTOMER_ID, user.MaKH),
                }),

                Expires = DateTime.UtcNow.AddMinutes(10), //Thời gian hết hạn

                //Xác thực chữ kí của token bằng khóa bí mật và thuật toán HMAC-SHA512.
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), 
                                                            SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);// tạo token
            var accessToken = jwtTokenHandler.WriteToken(token);//chuyển đổi đối tượng token thành một chuỗi JWT.
            var refreshToken = GenerateRefreshToken();// tạo một refresh token mới.
            _logger.LogInformation("Đã tạo refresh token , access token, token descriptor,  cho khách hàng: {UserId}", user.MaKH);
            //Tạo bảng để chứa refresh token trong database 
            //Lưu accessToken vao database
            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                JwtId = token.Id,
                MaKH = user.MaKH,
                Token = refreshToken,
                IsUsed = false,
                IsRevoked = false,
                IssuedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddHours(1),
            };

            await _refreshToken.AddAsync(refreshTokenEntity);
            _logger.LogInformation("Đã lưu refresh token vào cơ sở dữ liệu cho khách hàng: {UserId}", user.MaKH);
            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };
        }
        //tạo mới token
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


        [HttpPost("renewToken")]
        public async Task<IActionResult> RenewToken(TokenModel tokenModel)
        {
            _logger.LogInformation("Bắt đầu xử lý renew token");
            //check xem token gửi lên nó còn hợp lệ không trước khi cấp phát một access token mới.
            var jwtTokenHandler = new JwtSecurityTokenHandler(); //sử dụng để tạo và viết token JWT.
            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey); //Chuyển đổi khóa bí mật thành mảng byte.

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
                //check 1: AccessToken valid format
                var tokenInverification = jwtTokenHandler.ValidateToken(tokenModel.AccessToken, tokenValidateParam, out validatedToken);
                //check 2: check alg
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    _logger.LogDebug("Xác thực AccessToken.");
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);
                    if (!result)
                    {
                        _logger.LogWarning("Token không hợp lệ: thuật toán không khớp.");
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Invalid token"
                        });
                    }
                }

                _logger.LogDebug("Kiểm tra accessToken hết hạn ?");
                //check 3: Check accessToken expire?
                var utcExpireDate = long.Parse(tokenInverification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expireDate = ConvertUnixTimeToDaateTime(utcExpireDate);

                if (expireDate > DateTime.UtcNow)
                {
                    _logger.LogWarning("Access token chưa hết hạn");
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Access token has not yet expired"
                    });
                }

                _logger.LogDebug("Kiểm tra tồn tại RefreshToken ");
                //check 4: check refreshtoken exist in DB
                var storedToken = await _refreshToken.GetTokenAsync(tokenModel.RefreshToken);
                if (storedToken is null)
                {
                    _logger.LogWarning("Refresh token không tồn tại");
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Refresh token doesn't exist"
                    });
                }

                _logger.LogDebug("Kiểm tra trạng thái token");
                //check 5: check refresh is used/ revoked ?
                if (storedToken.IsUsed)
                {
                    _logger.LogWarning("Refresh token đã được sử dụng");
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Refresh token has been exist"
                    });
                }
                if (storedToken.IsRevoked)
                {
                    _logger.LogWarning("Refresh token đã bị thu hồi");
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Refresh token has been Revoked"
                    });
                }

                //check 6: AccessToken ID = JwID in RefreshToken // dịch ngược lại để lấy JwtId từ chuỗi token
                var jti = tokenInverification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                _logger.LogDebug("Kiểm tra token có khớp với JWT ID trong cơ sở dữ liệu.");
                if (storedToken.JwtId != jti)
                {
                    _logger.LogWarning("Token không khớp");
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Token doesn't match"
                    });
                }
                _logger.LogDebug("Cập nhật trạng thái của RefreshToken");
                //check 7: update token is used
                storedToken.IsRevoked = true;
                storedToken.IsUsed = true;
                await _refreshToken.UpdateAsync(storedToken, tokenModel.RefreshToken);

                //create new token
                var user = await _khachHang.GetUserById(storedToken.MaKH);
                var token = await GenerateToken(user);
                _logger.LogInformation("Tạo mới token thành công.");
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Renew Token Success",
                    Data = token
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý renew token.");
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Something went wrong"
                });
            }

        }

        private DateTime ConvertUnixTimeToDaateTime(long utcExpireDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();
            return dateTimeInterval;
        }
    }
}

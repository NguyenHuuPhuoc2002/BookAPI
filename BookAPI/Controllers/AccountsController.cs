using BookAPI.Common;
using BookAPI.Data;
using BookAPI.Helper;
using BookAPI.Models;
using BookAPI.Repositories;
using BookAPI.Repositories.Interfaces;
using BookAPI.Services;
using BookAPI.Services.Interfaces;
using Common.Exceptions;
using Google.Apis.Auth.OAuth2;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
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
        private readonly IGoogleService _googleService;
        private readonly IMailService _email;
        private readonly IResponseCacheService _responseCacheService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly GoogleAuthSettings _googleAuthSettings;
        private readonly FacebookAuthSettings _facebookAuthOptions;
        private readonly IFacebookService _facebookService;
        private SecurityToken validatedToken;

        public AccountsController(IAccountService account, IConfiguration configuration, IRefreshTokenService refreshToken, IGioHangService cart, 
                                   IOptionsMonitor<AppSetting> optionsMonitor, ILogger<AccountsController> logger, IOptions<FacebookAuthSettings> facebookAuthOptions,
                                   IMailService mail, UserManager<ApplicationUser> userManager, IResponseCacheService responseCacheService,
                                   IGoogleService googleService, IOptions<GoogleAuthSettings> googleAuthOptions, IFacebookService facebookService)
        {
            _cart = cart;
            _account = account;
            _configuration = configuration;
            _refreshToken = refreshToken;
            _appSettings = optionsMonitor.CurrentValue;
            _logger = logger;
            _googleService = googleService;
            _email = mail;
            _responseCacheService = responseCacheService;
            _userManager = userManager;
            _googleAuthSettings = googleAuthOptions.Value;
            _facebookAuthOptions = facebookAuthOptions.Value;
            _facebookService = facebookService;
        }

        [HttpGet("login-by-facebook")]
        [AllowAnonymous]
        public IActionResult LoginByFacebook()
        {
            var redirectUri = Url.Action("FacebookResponse", "Accounts", null, Request.Scheme);
            var facebookAuthUrl = _facebookService.GetFacebookAuthUrl(redirectUri);
            return Ok(new 
            {
                Success = true,
                Url = facebookAuthUrl
            });
        }

        [HttpGet("FacebookResponse")]
        public async Task<IActionResult> FacebookResponse([FromQuery] string code)
        {
            //Ckeck code null ?
            if (string.IsNullOrEmpty(code))
            {
                return Unauthorized(new ApiResponse
                {
                    Success = false,
                    Message = "Không nhận được mã xác thực từ Google!"
                });
            }

            // Gửi yêu cầu đến Google để đổi mã code lấy access_token
            using var httpClient = new HttpClient();
            var tokenRequest = new Dictionary<string, string>
            {
                { "client_id", _facebookAuthOptions.ClientId},
                { "client_secret", _facebookAuthOptions.ClientSecret},
                { "code", code },
                { "grant_type", "authorization_code" },
                { "redirect_uri", Url.Action("FacebookResponse", "Accounts", null, Request.Scheme) }
            };
            var response = await httpClient.PostAsync("https://graph.facebook.com/v18.0/oauth/access_token", new FormUrlEncodedContent(tokenRequest));

            var jsonResponse = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                return Unauthorized(new ApiResponse
                {
                    Success = false,
                    Message = "Lỗi xác thực Google!",
                    Data = jsonResponse
                });
            }

            // Parse kết quả
            var tokenData = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponse);
            var accessToken = tokenData["access_token"];

            var userInfoResponse = await httpClient.GetStringAsync(
                    $"https://graph.facebook.com/me?fields=id,first_name,last_name,email&access_token={accessToken}");

            var userInfo = JsonConvert.DeserializeObject<Dictionary<string, object>>(userInfoResponse);
            var email = userInfo["email"].ToString();
            var firstName = userInfo["first_name"].ToString();
            var lastName = userInfo["last_name"].ToString();

            var existingUser = await _account.FindByEmailAsync(email);

            //nếu = null thì đăng kí tài khoản
            if (existingUser == null)
            {
                var newUser = new SignUpModel
                {
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    DayOfBirth = null,
                    Hinh = null,
                    Password = "abc@ABC123",
                    ConfirmPassword = "abc@ABC123",
                    Gender = null,
                };

                var createResult = await _account.SignUpAsync(newUser);
                if (createResult == null)
                {
                    return BadRequest(new ApiResponse
                    {
                        Message = "Không tạo được tài khoản !"
                    });
                }
            }

            //Kiểm tra và đăng nhập user
            var loginModel = new SignInModel { Email = email, Password = "abc@ABC123" };

            var user = await _account.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound(new ApiResponse
                {
                    Message = "Không tìm thấy tài khoản!"
                });
            }

            bool isPasswordValid = await _userManager.CheckPasswordAsync(user, loginModel.Password);

            if (!isPasswordValid)
            {
                return BadRequest(new ApiResponse
                {
                    Message = "Sai thông tin đăng nhập!"
                });
            }
            var token = await GenerateToken(user);
            var cachedToken = await _responseCacheService.GetCacheResponseAsync(token.AccessToken);

            return Ok(token);
        }

        [HttpGet("login-by-google")]
        [AllowAnonymous]
        public IActionResult LoginByGoogle()
        {
            var redirectUri = Url.Action("GoogleResponse", "Accounts", null, Request.Scheme);
            var googleAuthUrl = _googleService.GetGoogleAuthUrl(redirectUri);
            return Ok(new { success = true, location = googleAuthUrl });
            
        }
     
        [HttpGet("GoogleResponse")]
        public async Task<IActionResult> GoogleResponse([FromQuery] string code)
        {
            //Ckeck code null ?
            if (string.IsNullOrEmpty(code))
            {
                return Unauthorized(new ApiResponse
                {
                    Success = false,
                    Message = "Không nhận được mã xác thực từ Google!" 
                });
            }

            // Gửi yêu cầu đến Google để đổi mã code lấy access_token
            using var httpClient = new HttpClient();
            var tokenRequest = new Dictionary<string, string>
            {
                { "client_id", _googleAuthSettings.ClientId },
                { "client_secret", _googleAuthSettings.ClientSecret },
                { "code", code },
                { "grant_type", "authorization_code" },
                { "redirect_uri", Url.Action("GoogleResponse", "Accounts", null, Request.Scheme) }
            };

            var response = await httpClient.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(tokenRequest));
            var jsonResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return Unauthorized(new ApiResponse 
                { 
                    Success = false,
                    Message = "Lỗi xác thực Google!", 
                    Data = jsonResponse 
                });
            }

            // Parse kết quả
            var tokenData = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponse);

            var accessToken = tokenData["access_token"];

            // Lấy thông tin người dùng từ access_token
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var userInfoResponse = await httpClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
            var userInfoJson = await userInfoResponse.Content.ReadAsStringAsync();
            var userInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(userInfoJson);

            //Lấy email, name từ json
            var email = userInfo["email"];
            var firstName = userInfo["given_name"];
            var lastName = userInfo["family_name"];
            var avartar = userInfo["picture"];
           
            //check emai, name null ?
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                return BadRequest(new ApiResponse
                {
                    Message = "email hoặc name là null !"
                });
            }

            //check email đã tồn tại trong db chưa
            var existingUser = await _account.FindByEmailAsync(email);

            //nếu = null thì đăng kí tài khoản
            if (existingUser == null)
            {
                var newUser = new SignUpModel
                {
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    DayOfBirth = null,
                    Hinh = null,
                    Password = "abc@ABC123",
                    ConfirmPassword = "abc@ABC123",
                    Gender = null,
                };

                var createResult = await _account.SignUpAsync(newUser);
                if (createResult == null)
                {
                    return BadRequest(new ApiResponse
                    {
                        Message = "Không tạo được tài khoản !"
                    });
                }
            }

            //Kiểm tra và đăng nhập user
            var loginModel = new SignInModel { Email = email, Password = "abc@ABC123" };

            var user = await _account.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound(new ApiResponse
                {
                    Message = "Không tìm thấy tài khoản!"
                });
            }

            bool isPasswordValid = await _userManager.CheckPasswordAsync(user, loginModel.Password);

            if (!isPasswordValid)
            {
                return BadRequest(new ApiResponse
                {
                    Message = "Sai thông tin đăng nhập!"
                });
            }
            var token = await GenerateToken(user);
            var cachedToken = await _responseCacheService.GetCacheResponseAsync(token.AccessToken);
            //GlobalVariables.email = user.UserName;
            //TokenGlobalVariable.Token = token.AccessToken;

            return Ok(token);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> SignOut([FromBody] LogOutRequestModel requestToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadToken(requestToken.AccessToken) as JwtSecurityToken;
            if (token == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Access Token không hợp lệ"
                });
            }
            var email = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(requestToken.AccessToken))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Access Token không hợp lệ"
                });
            }

            var dataRequest = new LogOutRequestModel
            {
                AccessToken = requestToken.AccessToken,
            };
            var cacheDataString = JsonConvert.SerializeObject(dataRequest);//Chuyển đổi dữ liệu token thành chuỗi JSON để lưu vào cache

            //Lấy thời gian hết hạn của Access Token
            var expires = token.ValidTo; 
            var remainingTime = expires - DateTime.UtcNow;

            var keyCache = $"{email}:{requestToken.AccessToken}"; 
            //await _responseCacheService.SetCacheReponseAsync(email, cacheDataString, remainingTime);
            await _responseCacheService.SetCacheReponseAsync(keyCache, cacheDataString, remainingTime);

            //Kiểm tra và thu hồi Refresh Token
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

        [HttpPost("login")]
        public async Task<IActionResult> SignIn(SignInModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new MissingFieldException("Nhập đầy đủ thông tin bắt buộc !");
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
                    _logger.LogWarning("Token chưa đã hết hạn");
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
                    _logger.LogWarning("Token không tồn tại trong csdl");
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
            return dateTimeInterval.AddSeconds(utcExpireDate);
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

using BookAPI.Services.Interfaces;

namespace BookAPI.Services
{

    public class GoogleService : IGoogleService
    {

        private readonly IConfiguration _configuration;

        public GoogleService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GetGoogleAuthUrl(string redirectUri)
        {

            var clientId = _configuration["Authentication:Google:ClientId"];
            var googleAuthUrl = $"https://accounts.google.com/o/oauth2/v2/auth?" +
                     $"client_id={clientId}" +
                     $"&response_type=code" +  // Lấy mã code thay vì token trực tiếp
                     $"&scope=openid%20profile%20email" +
                     $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                     $"&state={Guid.NewGuid()}"; // Tránh tấn công CSRF
            return googleAuthUrl;
        }
    }
}

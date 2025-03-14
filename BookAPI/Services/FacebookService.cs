using BookAPI.Services.Interfaces;
using System;

namespace BookAPI.Services
{
    public class FacebookService : IFacebookService
    {
        private readonly IConfiguration _configuration;

        public FacebookService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GetFacebookAuthUrl(string redirectUri)
        {
            var clientId = _configuration["Authentication:Facebook:ClientId"];
            var facebookAuthUrl = $"https://www.facebook.com/v18.0/dialog/oauth?" +
                                  $"client_id={clientId}" +
                                  $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                                  $"&scope=email,public_profile" +
                                  $"&state={Guid.NewGuid()}" +
                                  $"&response_type=code";
            return facebookAuthUrl;
        }
    }
}

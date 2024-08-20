using BookAPI.Data;
using BookAPI.Repositories.Interfaces;
using BookAPI.Services.Interfaces;

namespace BookAPI.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshToken;

        public RefreshTokenService(IRefreshTokenRepository refreshToken)
        {
            _refreshToken = refreshToken;
        }
        public async Task AddAsync(RefreshToken token)
        {
            await _refreshToken.AddAsync(token);
        }

        public async Task<RefreshToken> GetTokenAsync(string refreshToken)
        {
            return await _refreshToken.GetTokenAsync(refreshToken);
        }

        public async Task UpdateAsync(RefreshToken token, string refreshToken)
        {
            await _refreshToken.UpdateAsync(token, refreshToken);
        }
    }
}

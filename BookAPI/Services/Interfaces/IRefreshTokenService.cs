using BookAPI.Data;

namespace BookAPI.Services.Interfaces
{
    public interface IRefreshTokenService
    {
        Task AddAsync(RefreshToken token);
        Task UpdateAsync(RefreshToken token, string refreshToken);
        Task<RefreshToken> GetTokenAsync(string refreshToken);
    }
}

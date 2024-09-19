using BookAPI.Data;
using BookAPI.Repositories.RepositoryBase;

namespace BookAPI.Repositories.Interfaces
{
    public interface IRefreshTokenRepository 
    {
        Task AddAsync(RefreshToken token);
        Task UpdateAsync(RefreshToken token, string refreshToken);
        Task<RefreshToken> GetTokenAsync(string refreshToken);
    }
}

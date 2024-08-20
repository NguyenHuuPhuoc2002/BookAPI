using BookAPI.Models;
using BookAPI.Repositories;
using BookAPI.Repositories.Interfaces;
using BookAPI.Services.Interfaces;

namespace BookAPI.Services
{
    public class SachService : ISachService
    {
        private readonly ISachRepository _sach;
        private readonly ILogger<SachService> _logger;

        public SachService(ISachRepository sach, ILogger<SachService> logger) 
        {
            _sach = sach;
            _logger = logger;
        }

        public async Task<IEnumerable<SachModel>> GetAllBooksAsync(string? maLoai, int page, int pageSize)
        {
            return await _sach.GetAllBooksAsync(maLoai, page, pageSize);
        }

        public async Task<SachModel> GetBookByIdAsync(string id)
        {
            return await _sach.GetBookByIdAsync(id);
        }

        public async Task<IEnumerable<SachModel>> SearchBookAsync(string key, int page, int pageSize)
        {
            return await _sach.SearchBookAsync(key, page, pageSize);
        }

        public async Task<IEnumerable<SachModel>> SearchBookByNXBAsync(string key, int page, int pageSize)
        {
            return await _sach.SearchBookByNXBAsync(key, page, pageSize);
        }
    }
}

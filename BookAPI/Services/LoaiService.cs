using BookAPI.Data;
using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using BookAPI.Services.Interfaces;

namespace BookAPI.Services
{
    public class LoaiService : ILoaiService
    {
        private readonly ILoaiRepository _loai;

        public LoaiService(ILoaiRepository loai) 
        {
            _loai = loai;
        }

        public async Task<bool> AddAsync(LoaiModel model)
        {
            return await _loai.AddAsync(model);
        }

        public async Task<IEnumerable<LoaiModel>> GetAllLoaiAsync()
        {
            return await _loai.GetAllLoaiAsync();
        }

        public async Task<LoaiModel> GetLoaiByIdAsync(string id)
        {
            return await _loai.GetLoaiByIdAsync(id);
        }

        public async Task<bool> RemoveAsync(string id)
        {
            return await _loai.RemoveAsync(id);
        }

        public async Task<bool> UpdateAsync(LoaiModel model)
        {
            return await _loai.UpdateAsync(model);
        }
    }
}

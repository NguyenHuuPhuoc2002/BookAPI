using BookAPI.Models;

namespace BookAPI.Services.Interfaces
{
    public interface IPublisherService
    {
        Task<IEnumerable<PublisherModel>> GetAllAsync(int page, int pageSize);
        Task<IEnumerable<PublisherModel>> SearchAsync(string key, int page, int pageSize);
        Task<PublisherModel> GetByIdAsync(int id);
        Task<PublisherModel> GetByNameAsync(string key);
        Task<bool> AddAsync(PublisherModel model);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateAsync(PublisherModel model);
    }
}

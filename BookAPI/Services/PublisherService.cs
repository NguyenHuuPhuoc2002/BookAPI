using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using BookAPI.Services.Interfaces;

namespace BookAPI.Services
{
    public class PublisherService : IPublisherService
    {
        private readonly IPublisherRepository _publisher;

        public PublisherService(IPublisherRepository publisher) 
        {
            _publisher = publisher;
        }
        public async Task<bool> AddAsync(PublisherModel model)
        {
            return await _publisher.AddAsync(model);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _publisher.DeleteAsync(id);
        }

        public async Task<IEnumerable<PublisherModel>> GetAllAsync(int page, int pageSize)
        {
            return await _publisher.GetAllAsync(page, pageSize);
        }

        public async Task<PublisherModel> GetByIdAsync(int id)
        {
            return await _publisher.GetByIdAsync(id);
        }

        public async Task<PublisherModel> GetByNameAsync(string key)
        {
            return await _publisher.GetByNameAsync(key);
        }

        public async Task<IEnumerable<PublisherModel>> SearchAsync(string key, int page, int pageSize)
        {
            return await _publisher.SearchAsync(key, page, pageSize);
        }

        public async Task<bool> UpdateAsync(PublisherModel model)
        {
            return await _publisher.UpdateAsync(model);
        }
    }
}

using Microsoft.Extensions.Caching.Memory;

namespace BookAPI.Services
{
    public class CacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void SetCache<T>(string key, T data, TimeSpan absoluteExpirationRelativeToNow, TimeSpan? slidingExpiration)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow,
                SlidingExpiration = slidingExpiration,
                Size = 512
            };

            _memoryCache.Set(key, data, cacheEntryOptions);
        }

        public T GetCache<T>(string key)
        {
            _memoryCache.TryGetValue(key, out T value);
            return value;
        }
        public void RemoveCache(string key)
        {
            _memoryCache.Remove(key);
        }
    }

}

using EmployeeMaintenance.DL.Services.BLL;
using EmployeeMaintenance.DL.ValueObjects;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeMaintenance.BLL.Helpers
{
    public class CacheHelper : ICacheHelper
    {
        private readonly IMemoryCache _memoryCache;

        public CacheHelper(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T GetOrCreate<T>(string cacheKey, Func<ICacheEntry, T> factoryMethod, CacheOptions? options = null)
        {
            return _memoryCache.GetOrCreate(cacheKey, cacheEntry =>
            {
                if (options != null)
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow;
                    cacheEntry.SlidingExpiration = options.SlidingExpiration;
                    cacheEntry.Priority = options.Priority;
                    cacheEntry.Size = options.Size;
                }
                return factoryMethod(cacheEntry);
            });
        }

        public async Task<T> GetOrCreateAsync<T>(string cacheKey, Func<ICacheEntry, Task<T>> factoryMethod, CacheOptions? options = null)
        {
            return await _memoryCache.GetOrCreateAsync(cacheKey, async cacheEntry =>
            {
                if (options != null)
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow;
                    cacheEntry.SlidingExpiration = options.SlidingExpiration;
                    cacheEntry.Priority = options.Priority;
                    cacheEntry.Size = options.Size;
                }
                return await factoryMethod(cacheEntry);
            });
        }

        public void Remove(string cacheKey)
        {
            _memoryCache.Remove(cacheKey);
        }
    }
}

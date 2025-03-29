using Microsoft.Extensions.Caching.Memory;

namespace ControlFloor.Services
{
    public class TokenCacheService
    {
        private readonly IMemoryCache _cache;

        public TokenCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void SetToken(string tokenKey, string token, int expirationMinutes)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(expirationMinutes))
                .SetPriority(CacheItemPriority.Normal);

            _cache.Set(tokenKey, token, cacheEntryOptions);
        }

        public string GetToken(string tokenKey)
        {
            _cache.TryGetValue(tokenKey, out string token);
            return token;
        }

        public void RemoveToken(string tokenKey)
        {
            _cache.Remove(tokenKey);
        }
    }
}

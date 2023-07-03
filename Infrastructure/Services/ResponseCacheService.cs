using Core.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDatabase database;
        public ResponseCacheService(IConnectionMultiplexer redis)
        {
            this.database = redis.GetDatabase();
        }

        public async Task CacheResponseAsync(string cacheKey, string response, TimeSpan timeToLive)
        {
            if (response is null)
                return;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var serializedResponse = JsonSerializer.Serialize(response, options);

            await this.database.StringSetAsync(cacheKey, serializedResponse, timeToLive);
            
        }

        public async Task<string> GetCachedResponse(string cacheKey)
        {
           var cachedResponse = await this.database.StringGetAsync(cacheKey);

            if (cachedResponse.IsNullOrEmpty)
                return null;

            return cachedResponse;
        }

    }
}

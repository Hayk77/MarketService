using StackExchange.Redis;

namespace MyProject.Services
{
    public class RedisCacheService
    {
        private readonly IDatabase _db;

        public RedisCacheService()
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false");
            _db = redis.GetDatabase();
        }

        public string Get(string key)
        {
            return _db.StringGet(key);
        }

        public void Set(string key, string value, TimeSpan? expiration = null)
        {
            if (expiration.HasValue)
            {
                _db.StringSet(key, value, expiration.Value);
            }
            else
            {
                _db.StringSet(key, value);
            }
        }

        public void Remove(string key)
        {
            _db.KeyDelete(key);
        }
    }
}
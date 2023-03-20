using System.Text.Json;
using StackExchange.Redis;

namespace CachingWebApi.Services
{
    public class CacheService : ICacheService
    {
        private IDatabase _chacheDb;
        public CacheService()
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            _chacheDb = redis.GetDatabase();
        }
        public T GetData<T>(string key)
        {
            var value = _chacheDb.StringGet(key);
            if(!string.IsNullOrEmpty(value))
                return JsonSerializer.Deserialize<T>(value);

            return default;
        }

        public object RemoveData(string key)
        {
            var _exits = _chacheDb.KeyExists(key);

            if(_exits)
                return _chacheDb.KeyDelete(key);

            return false;
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var expirtyTime = expirationTime.DateTime.Subtract(DateTime.Now);
            return _chacheDb.StringSet(key, JsonSerializer.Serialize(value), expirtyTime);
        }
    }
}
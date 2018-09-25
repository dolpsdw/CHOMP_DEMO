using System;
using Newtonsoft.Json;
using NFX.ApplicationModel.Pile;
using StackExchange.Redis;
namespace CHOMP_DEMO.Controllers
{
    public class CacheManager : ICacheManager, IDisposable
    {
        private LocalCache _cacheDirector = new LocalCache();
        private ConnectionMultiplexer _redis = ConnectionMultiplexer.Connect("localhost");
        private IDatabase _db;
        private ISubscriber _sub;
        public CacheManager()
        {
            _cacheDirector.Pile = new DefaultPile(_cacheDirector);
            _cacheDirector.PileAllocMode = AllocationMode.ReuseSpace; //_cacheDirector.Configure(null);
            _cacheDirector.Start();
            //Redis
            _db = _redis.GetDatabase();
        }

        public T Get<T>(string key)
        {
            object localCached =_cacheDirector.GetTable<string>(typeof(T).FullName).Get(key);
            if (localCached != null)
            {
                return (T) localCached;
            }

            RedisValue rCached = _db.StringGet(key);
            return rCached == RedisValue.Null ? default(T) : JsonConvert.DeserializeObject<T>(rCached);
        }

        public bool Set<T>(string key, T data)
        {
            var putResult =_cacheDirector.GetOrCreateTable<string>(typeof(T).FullName).Put(key, data, TimeSpan.FromMinutes(30).Seconds);
            bool rResult = _db.StringSet(key, JsonConvert.SerializeObject(data), TimeSpan.FromMinutes(30));
            return (putResult != PutResult.Collision && rResult);
        }

        public void Dispose()
        {
            _cacheDirector.WaitForCompleteStop();
            _cacheDirector?.Dispose();
            _redis.Dispose();
        }

        public class Persona
        {
            public string name;
        }
    }
}
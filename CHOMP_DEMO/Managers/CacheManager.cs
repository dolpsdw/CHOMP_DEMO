using System;
using NFX.ApplicationModel.Pile;

namespace CHOMP_DEMO.Controllers
{
    public class CacheManager : ICacheManager, IDisposable
    {
        private LocalCache _cacheDirector = new LocalCache();
        public CacheManager()
        {
            _cacheDirector.Pile = new DefaultPile(_cacheDirector);
            _cacheDirector.PileAllocMode = AllocationMode.ReuseSpace; //_cacheDirector.Configure(null);
            _cacheDirector.Start();
        }

        public T Get<T>(string key)
        {
            try
            {
                return (T) _cacheDirector.GetTable<string>(typeof(T).FullName).Get(key);
            }
            catch (Exception e)
            {
                return default(T);
            }
        }

        public bool Set<T>(string key, T data)
        {
            var putResult =_cacheDirector.GetOrCreateTable<string>(typeof(T).FullName).Put(key, data, TimeSpan.FromMinutes(30).Seconds);
            return (putResult != PutResult.Collision);
        }

        public void Dispose()
        {
            _cacheDirector.WaitForCompleteStop();
            _cacheDirector?.Dispose();
        }

        public class Persona
        {
            public string name;
        }
    }
}
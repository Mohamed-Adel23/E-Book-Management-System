using EBMS.Infrastructure.IServices.ICache;
using System.Runtime.Caching;

namespace EBMS.Data.Services.Cache
{
    public class CacheService : ICacheService
    {
        private ObjectCache _memoryCache = MemoryCache.Default;


        public T GetData<T>(string key)
        {
            try
            {
                T data = (T)_memoryCache.Get(key);
                return data;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public bool IsCached(string key)
        {
            try
            {
                if(_memoryCache.Contains(key))
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool SetData<T>(string key, T data, DateTime expirationTime)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                    return false;

                _memoryCache.Set(key, data, expirationTime);

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public object RemoveData(string key)
        {
            try
            {
                var result = _memoryCache.Remove(key);

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

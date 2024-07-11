namespace EBMS.Infrastructure.IServices.ICache
{
    public interface ICacheService
    {
        T GetData<T>(string key);
        bool IsCached(string key);
        bool SetData<T>(string key, T data, DateTime expirationTime);
        object RemoveData(string key);
    }
}

namespace EBMS.Infrastructure.IServices
{
    public interface IBaseService<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
    }
}

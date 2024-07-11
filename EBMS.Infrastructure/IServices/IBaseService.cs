using System.Linq.Expressions;

namespace EBMS.Infrastructure.IServices
{
    public interface IBaseService<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync(string[] includes = null!);
        T GetFirstByPredicate(Expression<Func<T, bool>> predicate, string[] includes = null!);
        IEnumerable<T> GetAllByPredicate(Expression<Func<T, bool>> predicate, string[] includes = null!);
    }
}

using EBMS.Data.DataAccess;
using EBMS.Infrastructure.IServices;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EBMS.Data.Services
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        protected BookDbContext _context;

        // The paremeter will come from UoW
        public BaseService(BookDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Get The Specified Entity By Its Id
        /// </summary>
        /// <param name="id">must be a number</param>
        /// <returns></returns>
        public async Task<T> GetByIdAsync(int id)
        {
            var result = await _context.Set<T>().FindAsync(id);

            return result;
        }

        /// <summary>
        /// Get The First Element By Specific condition
        /// </summary>
        /// <returns>An Object from entity that meets the condition</returns>
        public T GetFirstByPredicate(Expression<Func<T, bool>> predicate, string[] includes = null!)
        {
            IQueryable<T> query = _context.Set<T>();
            if (query == null)
                return null!;

            if(includes is not null)
                foreach (var include in includes)
                    query = query.Include(include);

            return query.FirstOrDefault(predicate);
        }

        /// <summary>
        /// Get All Elements By Specific condition
        /// </summary>
        /// <returns>The list of all objects that meet the condition</returns>
        public IEnumerable<T> GetAllByPredicate(Expression<Func<T, bool>> predicate, string[] includes = null!)
        {
            IQueryable<T> query = _context.Set<T>();
            if (query == null)
                return null!;

            if (includes is not null)
                foreach (var include in includes)
                    query = query.Include(include);

            return query.Where(predicate);
        }

        /// <summary>
        /// Get All Entity Objects 
        /// </summary>
        /// <returns>All Entity Objects </returns>
        public async Task<IEnumerable<T>> GetAllAsync(string[] includes = null!)
        {
            IQueryable<T> query = _context.Set<T>();
            if (query == null)
                return null!;

            if (includes is not null)
                foreach (var include in includes)
                    query = query.Include(include);

            return await query.ToListAsync();
        }
    }
}

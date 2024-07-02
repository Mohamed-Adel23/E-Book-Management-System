using EBMS.Data.DataAccess;
using EBMS.Infrastructure.IServices;
using Microsoft.EntityFrameworkCore;

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
        /// Get All Entity Objects 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }
    }
}

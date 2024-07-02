using EBMS.Infrastructure.DTOs.Book;
using EBMS.Infrastructure.Models;

namespace EBMS.Infrastructure.IServices
{
    public interface IBookService : IBaseService<Book>
    {
        Task<BookDTO> CreateAsync(BookModel model);
        Task<BookDTO> GetBookByIdAsync(int id);
        Task<IEnumerable<BookDTO>> GetAllBooksAsync();
        Task<BookDTO> UpdateAsync(int id, BookModel model);
        Task<bool> DeleteAsync(int id);

        // Features
        Task<IEnumerable<BookDTO>> GetAuthorBooksAsync(int id);
        Task<IEnumerable<BookDTO>> GetCategoryBooksAsync(int id);
    }
}

using EBMS.Infrastructure.DTOs.Book;
using EBMS.Infrastructure.Helpers;
using EBMS.Infrastructure.Models;
using EBMS.Infrastructure.Queries;

namespace EBMS.Infrastructure.IServices
{
    public interface IBookService : IBaseService<Book>
    {
        Task<BookDTO> CreateAsync(BookModel model);
        Task<BookDTO> GetBookByIdAsync(int id);
        Task<IEnumerable<BookDTO>> GetAllBooksAsync();
        Task<BookDTO> UpdateAsync(int id, BookModel model);
        Task<bool> DeleteAsync(int id);
        Task<DownloadFile> DownloadAsync(string curUserId, int id);

        // Features
        Task<IEnumerable<BookDTO>> GetAuthorBooksAsync(int id);
        Task<IEnumerable<BookDTO>> GetCategoryBooksAsync(int id);
        Task<IEnumerable<BookDTO>> GetBooksByPublicationDateRangeAsync(string from, string to);
        Task<IEnumerable<BookDTO>> GetBooksByRateAsync(decimal rate);
        Task<IEnumerable<BookDTO>> SearchAsync(string query);

        // Searching, Sorting, Pagination
        Task<PagedList<BookDTO>> FilterBooksAsync(GetBookQueries request);
    }
}

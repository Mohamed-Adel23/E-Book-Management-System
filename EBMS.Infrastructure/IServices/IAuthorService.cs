using EBMS.Infrastructure.DTOs.Author;
using EBMS.Infrastructure.Models;
namespace EBMS.Infrastructure.IServices
{
    public interface IAuthorService : IBaseService<Author>
    {
        Task<AuthorDTO> CreateAsync(AuthorModel model);
        Task<AuthorDTO> GetAuthorByIdAsync(int id);
        Task<IEnumerable<AuthorDTO>> GetAllAuthorsAsync();
        Task<AuthorDTO> UpdateAsync(int id, AuthorModel model);
        Task<bool> DeleteAsync(int id);
    }
}

using EBMS.Data.DataAccess;
using EBMS.Infrastructure.DTOs.Author;
using EBMS.Infrastructure.DTOs.Book;
using EBMS.Infrastructure.IServices;
using EBMS.Infrastructure.Models;

namespace EBMS.Data.Services
{
    public class AuthorService : BaseService<Author>, IAuthorService
    {
        // Inject The Context
        public AuthorService(BookDbContext context) : base(context) { }

        public async Task<AuthorDTO> CreateAsync(AuthorModel model)
        {
            var result = new AuthorDTO();
            // Store The Data in a new Author
            var author = new Author()
            {
                FullName = model.FullName,
                Bio = model.Bio,
                Created_at = DateTime.Now
            };
            // Check if there is an Author picture
            if (model.ProfilePic is not null)
            {
                // Check the allowed extensions
                var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg" };
                var fileExtension = Path.GetExtension(model.ProfilePic.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    result.Message = $"Profile Image should have one of the following extensions {string.Join(",", allowedExtensions)}";
                    return result;
                }
                var fileLength = 3 * 1024 * 1024;
                if (model.ProfilePic.Length > fileLength)
                {
                    result.Message = $"File exceeds size limit: {fileLength / (1024 * 1024)}M, choose another one with less size!";
                    return result;
                }
                using var dataStream = new MemoryStream();
                await model.ProfilePic.CopyToAsync(dataStream);
                author.ProfilePic = dataStream.ToArray();
            }
            
            // Add The Author to Database
            await _context.Authors.AddAsync(author);
            // SaveChanges to Database to get The Id
            await _context.SaveChangesAsync();
            // Stor Data to DTO
            result = AuthorDataDTO(author);

            return result;
        }

        public async Task<AuthorDTO> GetAuthorByIdAsync(int id)
        {
            var result = new AuthorDTO();
            // Check if The id is valid
            var author = await GetByIdAsync(id);
            if(author is null)
            {
                result.Message = "Author is not Found!";
                return result;
            }

            result = AuthorDataDTO(author);

            return result;
        }

        public async Task<IEnumerable<AuthorDTO>> GetAllAuthorsAsync()
        {
            var result = new List<AuthorDTO>();
            var authors = await GetAllAsync();
            
            foreach (var author in authors)
                result.Add(AuthorDataDTO(author));

            return result;
        }

        public async Task<AuthorDTO> UpdateAsync(int id, AuthorModel model)
        {
            var result = new AuthorDTO();
            // Check if The id is valid
            var author = await GetByIdAsync(id);
            if (author is null)
            {
                result.Message = "Author is not Found!";
                return result;
            }

            // Update Data
            author.FullName = model.FullName;
            author.Bio = model.Bio;
            author.Updated_at = DateTime.Now;
            // Check if there is an Author picture
            if (model.ProfilePic is not null)
            {
                // Check the allowed extensions
                var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg" };
                var fileExtension = Path.GetExtension(model.ProfilePic.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    result.Message = $"Profile Image should have one of the following extensions {string.Join(",", allowedExtensions)}";
                    return result;
                }
                var fileLength = 3 * 1024 * 1024;
                if (model.ProfilePic.Length > fileLength)
                {
                    result.Message = $"File exceeds size limit: {fileLength / (1024 * 1024)}M, choose another one with less size!";
                    return result;
                }
                using var dataStream = new MemoryStream();
                await model.ProfilePic.CopyToAsync(dataStream);
                author.ProfilePic = dataStream.ToArray();
            }

            // Update the Author to Memory
            _context.Authors.Update(author);
            // Add Data To DTO
            result = AuthorDataDTO(author);

            return result;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // Check if id is valid
            var author = await GetByIdAsync(id);
            if(author is null) 
                return false;

            // Remove Author From Memory
            _context.Authors.Remove(author);

            return true;
        }


        private AuthorDTO AuthorDataDTO(Author author)
        {
            var result = new AuthorDTO();

            result.Id = author.Id;
            result.FullName = author.FullName;
            result.Bio = author.Bio;
            result.ProfilePic = author.ProfilePic;
            result.Created_at = author.Created_at;
            result.Updated_at = author.Updated_at;

            return result;
        }
    }
}

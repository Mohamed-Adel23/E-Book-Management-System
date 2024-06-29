using EBMS.Infrastructure.DTOs.Category;
using EBMS.Infrastructure.Models;

namespace EBMS.Infrastructure.IServices
{
    public interface ICategoryService: IBaseService<Category>
    {
        Task<CategoryDTO> CreateAsync(CategoryModel model); // Create New Category
        Task<CategoryDTO> GetCatByIdAsync(int id); // Get Category By id
        Task<IEnumerable<CategoryDTO>> GetAllCatAsync(); // Get All Category
        Task<CategoryDTO> UpdateAsync(int id, CategoryModel model); // Update Category
        Task<bool> DeleteAsync(int id); // Remove Category
    }
}

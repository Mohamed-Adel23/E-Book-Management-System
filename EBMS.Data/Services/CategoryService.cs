using EBMS.Data.DataAccess;
using EBMS.Infrastructure.DTOs.Category;
using EBMS.Infrastructure.IServices;
using EBMS.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace EBMS.Data.Services
{
    public class CategoryService : BaseService<Category>, ICategoryService
    {
        public CategoryService(BookDbContext context) : base(context) { }


        public async Task<CategoryDTO> CreateAsync(CategoryModel model)
        {
            var result = new CategoryDTO();
            // Check if the category is found 
            if(await _context.Categories.AnyAsync(x => x.Title == model.Title))
            {
                result.Message = "Category already exists!";
                return result;
            }

            // Create new Category to save it to database
            var newCat = new Category()
            {
                Title = model.Title,
                Description = model.Description,
                Created_at = DateTime.Now,
            };
            // Add to database
            await _context.Categories.AddAsync(newCat);
            // To Access the Id of the new category I should Save Changes to database
            await _context.SaveChangesAsync();

            // Add data to CategoryDTO
            result = CatDTO(newCat);

            return result;
        }

        public async Task<CategoryDTO> GetCatByIdAsync(int id)
        {
            var result = new CategoryDTO();
            var cat = await GetByIdAsync(id);
            // Check if there is not category with this id
            if(cat is null)
            {
                result.Message = "Category is not Found!";
                return result;
            }
            // Result Data
            result = CatDTO(cat);

            return result;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCatAsync()
        {
            var result = new List<CategoryDTO>();
            var cats = await GetAllAsync();

            foreach(var cat in cats)
            {
                result.Add(CatDTO(cat));
            }

            return result;
        }

        public async Task<CategoryDTO> UpdateAsync(int id, CategoryModel model)
        {
            var result = new CategoryDTO();
            var cat = await GetByIdAsync(id);
            // Check if there is not category with this id
            if (cat is null)
            {
                result.Message = "Category is not Found!";
                return result;
            }
            // Check if the Title exists
            if(cat.Title != model.Title)
            {
                if(await _context.Categories.AnyAsync(x => x.Title == model.Title))
                {
                    result.Message = "Category already exists!";
                    return result;
                }
            }
            // Change Data
            cat.Title = model.Title;
            cat.Description = model.Description;
            cat.Updated_at = DateTime.Now;
            // Update category into memory
            _context.Categories.Update(cat);
            // Result Data
            result = CatDTO(cat);

            return result;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cat = await GetByIdAsync(id);
            // Check if there is not category with this id
            if (cat is null)
                return false;

            // Remove The Category
            _context.Categories.Remove(cat);

            return true;
        }


        private CategoryDTO CatDTO(Category model)
        {
            var result = new CategoryDTO();

            result.Id = model.Id;
            result.Title = model.Title;
            result.Description = model.Description;
            result.Created_at = model.Created_at;
            result.Updated_at = model.Updated_at;
            
            return result;
        }
    }
}

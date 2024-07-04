using EBMS.Infrastructure;
using EBMS.Infrastructure.DTOs.Category;
using EBMS.Infrastructure.Helpers.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EBMS.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{RolesConstants.SuperAdmin},{RolesConstants.Admin}")]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost(Name = "createCat")]
        public async Task<IActionResult> CreateCatAsync(CategoryModel model)
        {
            if(!ModelState.IsValid) 
                return BadRequest(ModelState);

            var result = await _unitOfWork.Categories.CreateAsync(model);

            if(!string.IsNullOrEmpty(result.Message))
                return BadRequest(result);

            // Save Changes to Database
            await _unitOfWork.CompleteAsync();

            string url = Url.Link("getCat", new { id = result.Id });

            return Created(url, result);
        }

        [HttpGet("{id:int}", Name = "getCat")]
        public async Task<IActionResult> GetCatByIdAsync(int id)
        {
            var result = await _unitOfWork.Categories.GetCatByIdAsync(id);

            if(!string.IsNullOrEmpty(result.Message))
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("getAll", Name = "getAllCats")]
        public async Task<IActionResult> GetAllCatsAsync()
        {
            var result = await _unitOfWork.Categories.GetAllCatAsync();

            if(result is null)
                return NotFound("There are not Categories Yet!");

            return Ok(result);
        }

        [HttpPut("{id:int}", Name = "updateCat")]
        public async Task<IActionResult> UpdateCatAsync(int id,  CategoryModel model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _unitOfWork.Categories.UpdateAsync(id, model);

            if(!string.IsNullOrEmpty(result.Message))
                return BadRequest(result);

            // save Changes into Database
            await _unitOfWork.CompleteAsync();

            return Ok(result);
        }

        [HttpDelete("{id:int}", Name = "deleteCat")]
        public async Task<IActionResult> DeleteCatAsync(int id)
        {
            var result = await _unitOfWork.Categories.DeleteAsync(id);

            if(!result)
                return NotFound("Category is Not Found!");

            // Save Changes into Database
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}

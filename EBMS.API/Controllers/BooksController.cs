﻿using EBMS.Infrastructure;
using EBMS.Infrastructure.DTOs.Book;
using EBMS.Infrastructure.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EBMS.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public BooksController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize(Roles = $"{RolesConstants.SuperAdmin},{RolesConstants.Admin}")]
        [HttpPost(Name = "createBook")]
        public async Task<IActionResult> CreateBookAsync(BookModel model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _unitOfWork.Books.CreateAsync(model);

            if(!string.IsNullOrEmpty(result.Message))
                return BadRequest(result);

            // Save Changes To Database
            await _unitOfWork.CompleteAsync();

            // Create Link of Created Book
            var url = Url.Link("getBook", new { id = result.Id });

            return Created(url, result);
        }

        [HttpGet("{id:int}", Name = "getBook")]
        public async Task<IActionResult> GetBookByIdAsync(int id)
        {
            var result = await _unitOfWork.Books.GetBookByIdAsync(id);

            if (!string.IsNullOrEmpty(result.Message))
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("getAll", Name = "getAllBooks")]
        public async Task<IActionResult> GetAllBooksAsync()
        {
            var result = await _unitOfWork.Books.GetAllBooksAsync();

            if(result is null)
                return NotFound("There are not Books Yet!");

            return Ok(result);
        }

        [Authorize(Roles = $"{RolesConstants.SuperAdmin},{RolesConstants.Admin}")]
        [HttpPut("{id:int}", Name = "updateBook")]
        public async Task<IActionResult> UpdateBookAsync(int id, BookModel model)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            var result = await _unitOfWork.Books.UpdateAsync(id, model);

            if(!string.IsNullOrEmpty(result.Message))
                return BadRequest(result);

            // Save Changes To Database
            await _unitOfWork.CompleteAsync();

            return Ok(result);
        }

        // This Action can be accessed only by Super Admin, because of its sensitivity
        [Authorize(Roles = $"{RolesConstants.SuperAdmin}")]
        [HttpDelete("{id:int}", Name = "deleteBook")]
        public async Task<IActionResult> DeleteBookAsync(int id)
        {
            var result = await _unitOfWork.Books.DeleteAsync(id);

            if (!result)
                return NotFound("Invalid Book Id!");

            // Save Changes To Database
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        // Features
        [HttpGet("Author/{id:int}", Name = "getAuthorBooks")]
        public async Task<IActionResult> GetAuthorBooksAsync(int id)
        {
            var result = await _unitOfWork.Books.GetAuthorBooksAsync(id);

            if (result is null)
                return NotFound("Author is not found!");

            if(result.Count() <= 0)
                return NotFound("Author doesn't have any book yet!");

            return Ok(result);
        }

        [HttpGet("Category/{id:int}", Name = "getCategoryBooks")]
        public async Task<IActionResult> GetCategoryBooksAsync(int id)
        {
            var result = await _unitOfWork.Books.GetCategoryBooksAsync(id);

            if (result is null)
                return NotFound("category is not found!");

            if (result.Count() <= 0)
                return NotFound("Category doesn't have any book yet!");

            return Ok(result);
        }
    }
}

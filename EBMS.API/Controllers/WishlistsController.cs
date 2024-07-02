using EBMS.Infrastructure;
using EBMS.Infrastructure.DTOs.Wishlist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EBMS.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class WishlistsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public WishlistsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost(Name = "addToWishlist")]
        public async Task<IActionResult> AddToWishlistAsync(WishlistModel model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var curUserId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _unitOfWork.Wishlists.AddToWishlistAsync(curUserId!, model);

            if(!string.IsNullOrEmpty(result.Message))
                return BadRequest(result);

            // Save Changes To Database
            await _unitOfWork.CompleteAsync();

            string url = Url.Link("getUserWishlist", new { });

            return Created(url, result);
        }

        [HttpGet(Name = "getUserWishlist")]
        public async Task<IActionResult> GetUserWishlistAsync()
        {
            var curUserId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _unitOfWork.Wishlists.GetUserWishlistAsync(curUserId!);

            if (result is null)
                return NotFound("No items in wishlist yet!");

            return Ok(result);
        }

        [HttpDelete(Name = "deleteFromWishlist")]
        public async Task<IActionResult> RemoveFromWishlistAsync(WishlistModel model)
        {
            var curUserId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _unitOfWork.Wishlists.RemoveFromWishlist(curUserId!, model);

            if (!result)
                return BadRequest("Something went wrong!");

            // Save Changes To Database
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}

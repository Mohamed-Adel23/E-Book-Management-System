using EBMS.Infrastructure;
using EBMS.Infrastructure.DTOs.Order;
using EBMS.Infrastructure.Helpers.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EBMS.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrdersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost(Name = "createNewOrder")]
        public async Task<IActionResult> CreateOrderAsync(OrderModel model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get The Current User Id
            var curUserId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _unitOfWork.Orders.CreateAsync(curUserId!, model);

            if(!string.IsNullOrEmpty(result.Message))
                return BadRequest(result);

            // Save Changes with UoW
            await _unitOfWork.CompleteAsync();

            // Generate The Link of the Order
            string url = Url.Link("getOrder", new { id = result.Id });

            return Created(url, result);
        }

        [HttpGet("{id:int}", Name = "getOrder")]
        public async Task<IActionResult> GetOrderByIdAsync(int id)
        {
            // Get The Current User Id
            var curUserId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _unitOfWork.Orders.GetOrderByIdAsync(curUserId!, id);

            if (!string.IsNullOrEmpty(result.Message)) 
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet(Name = "getUserOrders")]
        public async Task<IActionResult> GetUserOrdersAsync()
        {
            // Get The Current User Id
            var curUserId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _unitOfWork.Orders.GetUserOrdersAsync(curUserId!);

            if(result is null)
                return BadRequest("Something went wrong!");

            return Ok(result);
        }

        [HttpGet("GetAll", Name = "getAllOrders")]
        [Authorize(Roles = $"{RolesConstants.SuperAdmin},{RolesConstants.Admin}")]
        public async Task<IActionResult> GetAllOrdersAsync()
        {
            var result = await _unitOfWork.Orders.GetAllOrdersAsync();

            if (result is null) 
                return BadRequest("No Orders Yet!");

            return Ok(result);
        }

        [HttpPut("{id:int}", Name = "updateOrder")]
        public async Task<IActionResult> UpdateAsync(int id, OrderModel model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get The Current User Id
            var curUserId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _unitOfWork.Orders.UpdateAsync(curUserId!, id, model);

            if(!string.IsNullOrEmpty(result.Message))
                return BadRequest(result);

            // Save Changes To Database
            await _unitOfWork.CompleteAsync();

            return Ok(result);
        }

        [HttpPut("cancel/{id:int}", Name = "cancelOrder")]
        public async Task<IActionResult> CancelOrderAsync(int id)
        {
            // Get The Current User Id
            var curUserId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _unitOfWork.Orders.CancelOrderAsync(curUserId!, id);

            if (!result)
                return BadRequest("Something went wrong!");

            // Save Changes To Database
            await _unitOfWork.CompleteAsync();

            return Ok("Order is canceled successfully!");
        }

        [HttpPut("delivered/{id:int}", Name = "deliveredOrder")]
        public async Task<IActionResult> DeliveredOrderAsync(int id)
        {
            // Get The Current User Id
            var curUserId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var result = await _unitOfWork.Orders.DeliveredOrderAsync(curUserId!, id);

            if (!result)
                return BadRequest("Something went wrong!");

            // Save Changes To Database
            await _unitOfWork.CompleteAsync();

            return Ok("Order marked as delivered successfully!");
        }

        // Payment Endpoint
        [HttpGet("Payment/{id:int}", Name = "orderPayment")]
        public async Task<IActionResult> OrderPaymentAsync(int id, string Cancel = null, string PayerID = "", string guid = "")
        {
            if (string.IsNullOrEmpty(PayerID))
            {
                // Get The Current User Id
                var curUserId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                string baseUrl = Request.Scheme + "://" + Request.Host + $"/Orders/Payment/{id}?";

                var redirectUrl = await _unitOfWork.PayWithPaypal.GetRedirectUrlAsync(curUserId, id, baseUrl);

                if (string.IsNullOrEmpty(redirectUrl))
                    return BadRequest("Something went wrong!");

                return Ok(redirectUrl);
            }
            else
            {
                var result = await _unitOfWork.PayWithPaypal.ExecutePaymentProcessAsync(id, PayerID);

                if (!result.IsCompleted)
                    return BadRequest(result);

                // Save Changes To Database
                await _unitOfWork.CompleteAsync();

                return Ok(result);
            }
        }
    }
}

using EBMS.Infrastructure.DTOs.Order;
using EBMS.Infrastructure.Models;

namespace EBMS.Infrastructure.IServices
{
    public interface IOrderService: IBaseService<Order>
    {
        Task<OrderDTO> CreateAsync(string curUserId, OrderModel model);
        Task<OrderDTO> GetOrderByIdAsync(string curUserId, int id);
        Task<IEnumerable<OrderDTO>> GetUserOrdersAsync(string curUserId);
        Task<IEnumerable<OrderDTO>> GetAllOrdersAsync();
        Task<OrderDTO> UpdateAsync(string curUserId, int orderId, OrderModel model);
        Task<bool> CancelOrderAsync(string curUserId, int id);
        Task<bool> DeliveredOrderAsync(string curUserId, int id);
    }
}

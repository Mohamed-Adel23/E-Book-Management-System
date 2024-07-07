
using EBMS.Infrastructure.DTOs.Payment;

namespace EBMS.Infrastructure.IServices.IPayment
{
    public interface IPaypalService
    {
        Task<string> GetRedirectUrlAsync(string curUserId, int orderId, string baseUrl);
        Task<PaymentDTO> ExecutePaymentProcessAsync(int orderId, string payerId);
    }
}

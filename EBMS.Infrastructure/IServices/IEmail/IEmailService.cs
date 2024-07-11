namespace EBMS.Infrastructure.IServices.IEmail
{
    public interface IEmailService
    {
        Task SendOrderEmail(int orderId, string subject, decimal total);
    }
}

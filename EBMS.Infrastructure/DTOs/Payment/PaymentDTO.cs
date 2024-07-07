namespace EBMS.Infrastructure.DTOs.Payment
{
    public class PaymentDTO : BaseDTO
    {
        public bool IsCompleted { get; set; }
        public int OrderId { get; set; }

        public string PayerId { get; set; }
        public string PaymentId { get; set; }

        public string Method { get; set; }
        public string Currency { get; set; }

        public decimal Amount { get; set; }

        public DateTime Paid_at { get; set; }
        public string Status { get; set; }
    }
}

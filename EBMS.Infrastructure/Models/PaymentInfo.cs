namespace EBMS.Infrastructure.Models
{
    public class PaymentInfo
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public string PayerId { get; set; } = null!;
        public string PaymentId { get; set; } = null!;

        public string Method { get; set; } = null!;
        public string Currency { get; set; } = null!;

        public decimal Amount { get; set; }

        public DateTime Paid_at { get; set; }
        public string Status { get; set; } = null!; // Completed || Not-Complete

    }
}

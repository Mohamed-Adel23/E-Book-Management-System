namespace EBMS.Infrastructure.DTOs.Order
{
    public class OrderDTO : BaseDTO
    {
        public string? UserName { get; set; }
        public string? ShipAddress { get; set; }
        public string? PostalCode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Status { get; set; } // Pending(Not-Paid), Delivering, Delivered, Canceled

        public List<OrderBook>? OrderBooks { get; set; } = new List<OrderBook>();

        public DateTime Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
    }
}

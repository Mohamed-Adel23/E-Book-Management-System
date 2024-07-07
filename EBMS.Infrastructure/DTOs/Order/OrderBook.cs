namespace EBMS.Infrastructure.DTOs.Order
{
    public class OrderBook
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}

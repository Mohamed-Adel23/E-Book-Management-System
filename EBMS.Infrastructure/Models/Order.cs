namespace EBMS.Infrastructure.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string ShipAddress { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        // Delivering(Paid + Ordered), Delivered(Paid + arrived),
        // Canceled(By User and the amount must be returned to User)
        public string Status { get; set; } = null!; 
        public DateTime Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
        // Has One User
        public string UserId { get; set; } = null!;
        public BookUser BookUser { get; set; } = null!;
        // Has Many Books
        public ICollection<Book> Books { get; set; } = new List<Book>();
        // Has Many BookOrders
        public ICollection<BookOrder> BookOrders { get; set; } = new List<BookOrder>();
    }
}

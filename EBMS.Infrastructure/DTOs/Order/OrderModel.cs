using System.ComponentModel.DataAnnotations;

namespace EBMS.Infrastructure.DTOs.Order
{
    public class OrderModel
    {
        [Required]
        public string ShipAddress { get; set; } = null!;
        [Required]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "The Postal Code must be exactly 6 numbers!")]
        public string PostalCode { get; set; } = null!;
        [Required]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "The Phone Number must be exactly 11 numbers!")]
        public string PhoneNumber { get; set; } = null!;
        [Required]
        public List<OrderBook> Books { get; set; } = null!;
    }
}

using EBMS.Data.DataAccess;
using EBMS.Infrastructure.DTOs.Order;
using EBMS.Infrastructure.Helpers.Constants;
using EBMS.Infrastructure.IServices;
using EBMS.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EBMS.Data.Services
{
    public class OrderService : BaseService<Order>, IOrderService
    {
        private readonly UserManager<BookUser> _userManager;
        public OrderService(BookDbContext context, UserManager<BookUser> userManager) : base(context)
        {
            _userManager = userManager;
        }

        public async Task<OrderDTO> CreateAsync(string curUserId, OrderModel model)
        {
            var result = new OrderDTO();

            // Check if the user Id is valid 
            var user = await _userManager.FindByIdAsync(curUserId);
            if (user is null)
            {
                result.Message = "User is not Found!";
                return result;
            }
            // To prevent Book Id duplications
            var bookIds = new HashSet<int>();
            // Check if Books Ids are valid 
            foreach (var detail in model.Books)
            {
                var book = await _context.Books.FindAsync(detail.BookId);
                if (book is null)
                {
                    result.Message = $"Invalid Book Id ({detail.BookId})";
                    return result;
                }
                // Check if the quantity in stock is enouph
                if (detail.Quantity > book.AvailableQuantity)
                {
                    result.Message = $"Quantity you ordred for book ({book.Id}) exceeds the available quantity: Available Quantity is ({book.AvailableQuantity})";
                    return result;
                }
                else if(detail.Quantity <= 0)
                {
                    result.Message = "Quantity must be greater than or equal to 1";
                    return result;
                }
                // Check if the book Id has been taken before!
                if (bookIds.Contains(detail.BookId))
                {
                    result.Message = $"This Book ({detail.BookId}) has been taken before!";
                    return result;
                } else
                {
                    bookIds.Add(detail.BookId);
                }
            }

            // Create New Order
            var newOrder = new Order()
            {
                ShipAddress = model.ShipAddress,
                PostalCode = model.PostalCode,
                PhoneNumber = model.PhoneNumber,
                UserId = curUserId,
                Created_at = DateTime.Now,
                Status = StatusConstants.Pending
            };
            // Save To Memory
            await _context.Orders.AddAsync(newOrder);
            // Save Changes To Database to Get The Order Id
            await _context.SaveChangesAsync();

            // Store Order Details into BookOrders
            var newBookOrders = new List<BookOrder>();
            foreach (var detail in model.Books)
            {
                var book = await _context.Books.FindAsync(detail.BookId);

                newBookOrders.Add(new BookOrder()
                {
                    OrderId = newOrder.Id,
                    BookId = detail.BookId,
                    Quantity = detail.Quantity,
                    UnitPrice = book!.PhysicalPrice
                });

                // Update The Quantity of the book
                book.AvailableQuantity = book.AvailableQuantity - detail.Quantity;
                _context.Books.Update(book);
                await _context.SaveChangesAsync();
            }
            // Save to Memory
            await _context.BookOrders.AddRangeAsync(newBookOrders);

            result = OrderDataDTO(newOrder, newBookOrders, user.UserName!);

            return result;
        }

        public async Task<OrderDTO> GetOrderByIdAsync(string curUserId, int id)
        {
            var result = new OrderDTO();

            var order = await _context.Orders.Include(x => x.BookOrders).SingleOrDefaultAsync(x => x.Id == id);
            // Check if the id is valid
            if (order is null)
            {
                result.Message = "Order is not found!";
                return result;
            }
            // Check if the current user owns this order & Admin and Super Admin Can access directly
            var user = await _userManager.FindByIdAsync(curUserId);
            if (user is null)
            {
                result.Message = "User is not found!";
                return result;
            }
            if(!await _userManager.IsInRoleAsync(user, RolesConstants.SuperAdmin) && !await _userManager.IsInRoleAsync(user, RolesConstants.Admin) && curUserId != order.UserId)
            {
                result.Message = "You can't access this order!";
                return result;
            }

            // Get The user of this order
            var orderUser = await _userManager.FindByIdAsync(order.UserId);
            result = OrderDataDTO(order, order.BookOrders.ToList(), orderUser?.UserName!);

            return result;
        }

        public async Task<IEnumerable<OrderDTO>> GetUserOrdersAsync(string curUserId)
        {
            var result = new List<OrderDTO>();

            // Check if the user Id valid
            var user = await _userManager.FindByIdAsync(curUserId);
            if (user is null)
                return null!;

            var orders = _context.Orders.Include(x => x.BookOrders).Where(x => x.UserId == curUserId);
            if (orders.Count() <= 0)
                return null!;

            foreach (var order in orders)
                result.Add(OrderDataDTO(order, order.BookOrders.ToList()));

            return result;
        }

        // This Action can be accessed only for Admins and Super Admin
        public async Task<IEnumerable<OrderDTO>> GetAllOrdersAsync()
        {
            var result = new List<OrderDTO>();

            var orders = await _context.Orders.Include(x => x.BookOrders).ToListAsync();
            if (orders.Count() <= 0)
                return null!;

            // Enhance the performance by memorize
            var userNames = new Dictionary<string, string>();
            foreach (var order in orders)
            {
                if (!userNames.ContainsKey(order.UserId))
                {
                    var user = await _userManager.FindByIdAsync(order.UserId);
                    userNames.Add(order.UserId, user?.UserName!);
                }
                result.Add(OrderDataDTO(order, order.BookOrders.ToList(), userNames[order.UserId]));
            }

            return result;
        }

        // Only The Current User can change his order data, if the status is Pending
        public async Task<OrderDTO> UpdateAsync(string curUserId, int orderId, OrderModel model)
        {
            var result = new OrderDTO();

            // Check the order Id
            var order = await _context.Orders.FindAsync(orderId);
            if(order is null)
            {
                result.Message = "Order is not found!";
                return result;
            }
            // Check if the current user owns this order
            var user = await _userManager.FindByIdAsync(curUserId);
            if(user is null)
            {
                result.Message = "User is not found!";
                return result;
            }
            // Check if the order is pending or not
            if (order.Status != StatusConstants.Pending || order.UserId != curUserId)
            {
                result.Message = "You can't update this order!";
                return result;
            }
            // 1- Delete All Details for this Order
            var orderBooks = _context.BookOrders.Where(x => x.OrderId == order.Id);
            foreach (var detail in orderBooks)
            {
                var book = await _context.Books.FindAsync(detail.BookId);
                // Update The Quantity of the book
                book.AvailableQuantity = book.AvailableQuantity + detail.Quantity;
                _context.Books.Update(book);

                // Remove detail
                _context.BookOrders.Remove(detail);
            }
            // Save Changes To Database
            await _context.SaveChangesAsync();

            // To prevent Book Id duplications
            var bookIds = new HashSet<int>();
            // Check if Books Ids are valid 
            foreach (var detail in model.Books)
            {
                var book = await _context.Books.FindAsync(detail.BookId);
                if (book is null)
                {
                    result.Message = $"Invalid Book Id ({detail.BookId})";
                    return result;
                }
                // Check if the quantity in stock is enouph
                if (detail.Quantity > book.AvailableQuantity)
                {
                    result.Message = $"Quantity you ordred for book ({book.Id}) exceeds the available quantity: Available Quantity is ({book.AvailableQuantity})";
                    return result;
                }
                else if (detail.Quantity <= 0)
                {
                    result.Message = "Quantity must be greater than or equal to 1";
                    return result;
                }
                // Check if the book Id has been taken before!
                if (bookIds.Contains(detail.BookId))
                {
                    result.Message = $"This Book ({detail.BookId}) has been taken before!";
                    return result;
                }
                else
                {
                    bookIds.Add(detail.BookId);
                }
            }

            // Update The Order
            order.ShipAddress = model.ShipAddress;
            order.PostalCode = model.PostalCode;
            order.PhoneNumber = model.PhoneNumber;
            order.Updated_at = DateTime.Now;
            // Update Data To Memory
            _context.Orders.Update(order);

            // 2- Add New Details
            var newBookOrders = new List<BookOrder>();
            foreach (var detail in model.Books)
            {
                var book = await _context.Books.FindAsync(detail.BookId);

                newBookOrders.Add(new BookOrder()
                {
                    OrderId = order.Id,
                    BookId = detail.BookId,
                    Quantity = detail.Quantity,
                    UnitPrice = book!.PhysicalPrice
                });

                // Update The Quantity of the book
                book.AvailableQuantity = book.AvailableQuantity - detail.Quantity;
                _context.Update(book);
                await _context.SaveChangesAsync();
            }
            // Add To Memory
            await _context.BookOrders.AddRangeAsync(newBookOrders);

            result = OrderDataDTO(order, newBookOrders);

            return result;
        }

        // The Owned User, Admin and Super Admin can access this method
        public async Task<bool> CancelOrderAsync(string curUserId, int id)
        {
            var order = await _context.Orders.FindAsync(id);
            // Check if the id is valid
            if (order is null)
                return false;

            // Check if the order is already canceled!
            if (order.Status == StatusConstants.Canceled)
                return false;

            // Check if the current user owns this order & Admin and Super Admin Can access directly
            var user = await _userManager.FindByIdAsync(curUserId);
            if (user is null)
                return false;
            if (!await _userManager.IsInRoleAsync(user, RolesConstants.SuperAdmin) && !await _userManager.IsInRoleAsync(user, RolesConstants.Admin) && curUserId != order.UserId)
                return false;

            order.Status = StatusConstants.Canceled;
            order.Updated_at = DateTime.Now;
            _context.Orders.Update(order);

            return true;
        }

        // The Owned User, Admin and Super Admin can access this method
        public async Task<bool> DeliveredOrderAsync(string curUserId, int id)
        {
            var order = await _context.Orders.FindAsync(id);
            // Check if the id is valid
            if (order is null)
                return false;

            // Check if the order is already Delivered!
            if (order.Status == StatusConstants.Delivered)
                return false;

            // Check if the current user owns this order & Admin and Super Admin Can access directly
            var user = await _userManager.FindByIdAsync(curUserId);
            if (user is null)
                return false;
            if (!await _userManager.IsInRoleAsync(user, RolesConstants.SuperAdmin) && !await _userManager.IsInRoleAsync(user, RolesConstants.Admin) && curUserId != order.UserId)
                return false;

            order.Status = StatusConstants.Delivered;
            order.Updated_at = DateTime.Now;
            _context.Orders.Update(order);

            return true;
        }



        private OrderDTO OrderDataDTO(Order order, List<BookOrder> details, string userName = null!)
        {
            var result = new OrderDTO();

            result.Id = order.Id;
            result.UserName = userName;
            result.ShipAddress = order.ShipAddress;
            result.PostalCode = order.PostalCode;
            result.PhoneNumber = order.PhoneNumber;
            result.Status = order.Status;
            result.Created_at = order.Created_at;
            result.Updated_at = order.Updated_at;

            foreach (var detail in details)
            {
                result.OrderBooks.Add(new OrderBook()
                {
                    BookId = detail.BookId,
                    Quantity = detail.Quantity,
                    UnitPrice = detail.UnitPrice
                });
            }

            return result;
        }
    }
}

using EBMS.Data.DataAccess;
using EBMS.Infrastructure.DTOs.Payment;
using EBMS.Infrastructure.Helpers.Constants;
using EBMS.Infrastructure.IServices.IPayment;
using EBMS.Infrastructure.Models;
using EBMS.Infrastructure.Models.Payment;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PayPal.Api;

namespace EBMS.Data.Services.Payment
{
    public class PaypalService : IPaypalService
    {
        private readonly BookDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConfiguration _configuration;
        private PayPal.Api.Payment _payment;

        public PaypalService(BookDbContext context, IHttpContextAccessor contextAccessor, IConfiguration configuration)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _configuration = configuration;
        }

        public async Task<string> GetRedirectUrlAsync(string curUserId, int orderId, string baseUrl)
        {
            var apiContext = GetApiContext();

            try
            {
                // Check if the order is valid 
                var order = await _context.Orders.Include(x => x.BookOrders).SingleOrDefaultAsync(x => x.Id == orderId);
                if (order is null)
                    return string.Empty;
                // Ensure that the current Order status is Pending
                if (order.Status != StatusConstants.Pending)
                    return string.Empty;
                // Check if the current user owns this order
                if (order.UserId != curUserId)
                    return string.Empty;

                var guid = Convert.ToString((new Random()).Next(100000));

                var createPayment = CreatePayment(apiContext, baseUrl + "guid=" + guid, order);

                var links = createPayment.links.GetEnumerator();
                
                string paypalredirectUrl = null!;
                while(links.MoveNext())
                {
                    Links link = links.Current;
                    if (link.rel.ToLower().Trim().Equals("approval_url"))
                        paypalredirectUrl = link.href;
                }

                _contextAccessor.HttpContext!.Session.SetString("paymentId", createPayment.id);

                return paypalredirectUrl!;
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task<PaymentDTO> ExecutePaymentProcessAsync(int orderId, string payerId)
        {
            var apiContext = GetApiContext();
            var result = new PaymentDTO();

            try
            {
                var paymentId = _contextAccessor.HttpContext.Session.GetString("paymentId");

                var executePayment = ExecutePayment(apiContext, payerId, paymentId);

                if (executePayment.state.ToLower() != "approved")
                {
                    result.Message = "Something went wrong!";
                    return result;
                }

                // Store Payment Data To PaymentInfo
                var newPayment = new PaymentInfo()
                {
                    PayerId = payerId,
                    PaymentId = paymentId,
                    OrderId = orderId,
                    Method = "Paypal",
                    Currency = "USD",
                    Paid_at = DateTime.Now,
                    Status = StatusConstants.Completed,
                    Amount = decimal.Parse(executePayment.transactions[0].amount.total)
                };
                // Save data to Memory
                await _context.AddAsync(newPayment);
                // Save To Database
                await _context.SaveChangesAsync();

                // Update The Status of Order to Delivering
                var order = await _context.Orders.FindAsync(orderId);
                if(order is null)
                {
                    result.Message = "Something went wrong!";
                    return result;
                }
                order.Status = StatusConstants.Delivering;
                order.Updated_at = DateTime.Now;
                // Update To Memory
                _context.Orders.Update(order);

                // Save Data To DTO
                result = PaymentDataDTO(newPayment);

                return result;
            }
            catch 
            {
                result.Message = "Something went wrong!";
                return result;
            }
        }



        private APIContext GetApiContext()
        {
            var clientId = _configuration.GetValue<string>("PayPal:ClientId");
            var clientSecret = _configuration.GetValue<string>("PayPal:ClientSecret");
            var mode = _configuration.GetValue<string>("PayPal:Mode");

            APIContext apiContext = PaypalConfiguration.GetAPIContext(clientId!, clientSecret!, mode!);

            return apiContext;
        }

        private PayPal.Api.Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this._payment = new PayPal.Api.Payment()
            {
                id = paymentId
            };

            return this._payment.Execute(apiContext, paymentExecution);
        }

        private PayPal.Api.Payment CreatePayment(APIContext apiContext, string redirectUrl, EBMS.Infrastructure.Models.Order order)
        {
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };

            decimal totalAmount = 0m;
            foreach (var item in order.BookOrders)
            {
                var book = _context.Books.Find(item.BookId);
                itemList.items.Add(new Item()
                {
                    name = book.Title,
                    currency = "USD",
                    price = item.UnitPrice.ToString(),
                    quantity = item.Quantity.ToString(),
                    sku = "EGY"
                });

                totalAmount += (item.Quantity * item.UnitPrice);
            }

            var payer = new Payer()
            {
                payment_method = "paypal"
            };

            var redirectUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };

            var amount = new Amount()
            {
                currency = "USD",
                total = totalAmount.ToString(),
            };

            var transactionList = new List<Transaction>();

            transactionList.Add(new Transaction()
            {
                description = "EBMS Order Transaction",
                invoice_number = Guid.NewGuid().ToString(),
                amount = amount,
                item_list = itemList
            });

            _payment = new PayPal.Api.Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirectUrls
            };


            return _payment.Create(apiContext);
        }

        private PaymentDTO PaymentDataDTO(PaymentInfo model)
        {
            var result = new PaymentDTO();

            result.IsCompleted = true;
            result.Message = "Sucessful payment!";
            result.Id = model.Id;
            result.PayerId = model.PayerId;
            result.PaymentId = model.PaymentId;
            result.OrderId = model.OrderId;
            result.Method = model.Method;
            result.Currency = model.Currency;
            result.Paid_at = model.Paid_at;
            result.Amount = model.Amount;
            result.Status = model.Status;

            return result;
        }
    }
}
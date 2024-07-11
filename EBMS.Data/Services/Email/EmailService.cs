using EBMS.Infrastructure.IServices.IEmail;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using EBMS.Data.DataAccess;
using Microsoft.AspNetCore.Identity;
using EBMS.Infrastructure.Models;
using EBMS.Infrastructure.Helpers.Constants;

namespace EBMS.Data.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly BookDbContext _context;
        private readonly UserManager<BookUser> _userManager;
        public EmailService(IConfiguration configuration, BookDbContext context, UserManager<BookUser> userManager)
        {
            _configuration = configuration;
            _context = context;
            _userManager = userManager;
        }

        public async Task SendOrderEmail(int orderId, string subject, decimal total)
        {
            var fMail = _configuration.GetSection("Email:EmailUsername").Value;
            var fPassword = _configuration.GetSection("Email:EmailPassword").Value;

            // Get necessary data
            var order = await _context.Orders.FindAsync(orderId);
            var user = await _userManager.FindByIdAsync(order!.UserId);
            var admins = await _userManager.GetUsersInRoleAsync(RolesConstants.Admin);

            string messageBody = EmailMessageBody(order, user!, total);
            var msg = new MailMessage();
            msg.From = new MailAddress(fMail!);
            msg.Subject = subject;
            msg.Body = $"<html><body>{messageBody}</body></html>";
            msg.IsBodyHtml = true;
            // Send This Email to all System Admins and SuperAdmin
            foreach (var admin in admins)
            {
                msg.To.Add(admin.Email!);
            }
            // Send Email to SuperAdmin
            var superAdmin = await _userManager.GetUsersInRoleAsync(RolesConstants.SuperAdmin);
            msg.To.Add(superAdmin.First().Email!);

            var smtp = new SmtpClient(_configuration.GetSection("Email:EmailHost").Value)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(fMail, fPassword),
                Port = 587
            };

            smtp.Send(msg);
        }


        private string EmailMessageBody(Order order, BookUser user, decimal total) =>
            $"<span style=\"color: green; font-weight: bold;\">NEW ORDER with ID = {order.Id}</span><br><br>Username: <b>{user.UserName}</b><br>Email: <b>{user.Email}</b><br>Address: <b>{order.ShipAddress}</b><br>Postal Code: <b>{order.PostalCode}</b><br>Status: <b>{order.Status}</b><br>Total Amount: <b>${total}</b><br><br><b>For More Details About this Order Check The Website.</b>";
        
    }
}

using EBMS.Data.DataAccess.Config;
using EBMS.Infrastructure.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace EBMS.Data.DataAccess
{
    public class BookDbContext : IdentityDbContext<BookUser>
    {
        public BookDbContext(DbContextOptions<BookDbContext> options) : base(options) { }

        // DbSets
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }
        public DbSet<BookOrder> BookOrders { get; set; }

        // Models Configurations
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(BookConfiguration).Assembly);
        }
    }
}

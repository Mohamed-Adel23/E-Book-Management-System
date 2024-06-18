using EBMS.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace EBMS.Data.DataAccess.Config
{
    public class BookOrderConfiguration : IEntityTypeConfiguration<BookOrder>
    {
        public void Configure(EntityTypeBuilder<BookOrder> builder)
        {
            builder.ToTable("BookOrders");
            builder.HasKey(x => new { x.BookId, x.OrderId });
            builder.Property(x => x.Quantity)
                   .HasColumnType("TINYINT")
                   .IsRequired();
            builder.Property(x => x.UnitPrice)
                   .HasColumnType("DECIMAL")
                   .HasPrecision(7, 2)
                   .IsRequired();
        }
    }
}

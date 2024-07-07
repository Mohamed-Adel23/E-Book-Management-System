using EBMS.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace EBMS.Data.DataAccess.Config
{
    public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
    {
        public void Configure(EntityTypeBuilder<Wishlist> builder)
        {
            builder.ToTable("Wishlists");
            builder.HasKey(x => new { x.BookId, x.UserId });
            builder.Property(x => x.Created_at)
                   .HasColumnType("DATETIME")
                   .IsRequired();
        }
    }
}

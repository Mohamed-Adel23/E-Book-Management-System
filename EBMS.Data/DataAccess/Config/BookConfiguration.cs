using EBMS.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace EBMS.Data.DataAccess.Config
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("Books");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd();
            builder.Property(x => x.Title)
                   .HasColumnType("NVARCHAR")
                   .HasMaxLength(255)
                   .IsRequired();
            builder.Property(x => x.Description)
                   .HasColumnType("NVARCHAR")
                   .HasMaxLength(255)
                   .IsRequired(false);
            builder.Property(x => x.Price)
                   .HasColumnType("DECIMAL")
                   .HasPrecision(7, 2)
                   .IsRequired();
            builder.Property(x => x.Discount)
                   .HasColumnType("DECIMAL")
                   .HasPrecision(5, 2)
                   .IsRequired();
            builder.Property(x => x.AvailableQuantity)
                   .HasColumnType("TINYINT")
                   .IsRequired();
            builder.Property(x => x.BookPath)
                   .HasColumnType("VARCHAR")
                   .HasMaxLength(255)
                   .IsRequired();
            builder.Property(x => x.CoverImage)
                   .HasColumnType("VARCHAR")
                   .HasMaxLength(255)
                   .IsRequired();
            builder.Property(x => x.Created_at)
                   .HasColumnType("DATETIME")
                   .HasDefaultValue(DateTime.UtcNow);
            builder.Property(x => x.Updated_at)
                   .HasColumnType("DATETIME")
                   .IsRequired(false);

            // Relationships
            // 1 Book => N Reviews
            builder.HasMany(x => x.Reviews)
                   .WithOne(x => x.Book)
                   .HasForeignKey(x => x.BookId)
                   .IsRequired();
            // 1 Book => N Wishlists
            builder.HasMany(x => x.Wishlists)
                   .WithOne(x => x.Book)
                   .HasForeignKey(x => x.BookId)
                   .IsRequired();
            // M Book => N Orders
            builder.HasMany(x => x.Orders)
                   .WithMany(x => x.Books)
                   .UsingEntity<BookOrder>(
                        l => l.HasOne(c => c.Order).WithMany(c => c.BookOrders).HasForeignKey(c => c.OrderId),
                        r => r.HasOne(c => c.Book).WithMany(c => c.BookOrders).HasForeignKey(c => c.BookId)
                    );
            // M Books => N Categories
            builder.HasMany(x => x.Categories)
                   .WithMany(x => x.Books)
                   .UsingEntity<BookCategory>(
                        l => l.HasOne(c => c.Category).WithMany(c => c.BookCategories).HasForeignKey(c => c.CategoryId),
                        r => r.HasOne(c => c.Book).WithMany(c => c.BookCategories).HasForeignKey(c => c.BookId)
                    );
        }
    }
}

using EBMS.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace EBMS.Data.DataAccess.Config
{
    public class BookUserConfiguration : IEntityTypeConfiguration<BookUser>
    {
        public void Configure(EntityTypeBuilder<BookUser> builder)
        {
            builder.Property(x => x.FullName)
                   .HasColumnType("NVARCHAR")
                   .HasMaxLength(255)
                   .IsRequired();
            builder.Property(x => x.Address)
                   .HasColumnType("NVARCHAR")
                   .HasMaxLength(255)
                   .IsRequired(false);
            builder.Property(x => x.Bio)
                   .HasColumnType("NVARCHAR")
                   .HasMaxLength(255)
                   .IsRequired(false);
            builder.Property(x => x.DateOfBirth)
                   .HasColumnType("DATE")
                   .IsRequired(false);
            builder.Property(x => x.AuthorFile)
                   .HasColumnType("VARCHAR")
                   .HasMaxLength(255)
                   .IsRequired();
            builder.Property(x => x.ProfilePic)
                   .HasColumnType("IMAGE")
                   .IsRequired();
            builder.Property(x => x.Created_at)
                   .HasColumnType("DATETIME")
                   .HasDefaultValue(DateTime.UtcNow);
            builder.Property(x => x.Updated_at)
                   .HasColumnType("DATETIME")
                   .IsRequired(false);

            // Relationships
            // 1 Author => N Books
            builder.HasMany(x => x.Books)
                   .WithOne(x => x.Author)
                   .HasForeignKey(x => x.AuthorId)
                   .IsRequired();
            // 1 User => N Reviews
            builder.HasMany(x => x.Reviews)
                   .WithOne(x => x.BookUser)
                   .HasForeignKey(x => x.UserId)
                   .IsRequired();
            // 1 User => N Wishlists
            builder.HasMany(x => x.Wishlists)
                   .WithOne(x => x.BookUser)
                   .HasForeignKey(x => x.UserId)
                   .IsRequired();
            // 1 User => N Orders
            builder.HasMany(x => x.Orders)
                   .WithOne(x => x.BookUser)
                   .HasForeignKey(x => x.UserId)
                   .IsRequired();
        }
    }
}

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
            builder.Property(x => x.Email)
                   .IsRequired();
            builder.Property(x => x.UserName)
                   .IsRequired();
            builder.Property(x => x.DateOfBirth)
                   .HasColumnType("DATE")
                   .IsRequired(false);
            builder.Property(x => x.ProfilePic)
                   .HasColumnType("IMAGE")
                   .IsRequired(false);
            builder.Property(x => x.Created_at)
                   .HasColumnType("DATETIME")
                   .IsRequired();
            builder.Property(x => x.Updated_at)
                   .HasColumnType("DATETIME")
                   .IsRequired(false);

            // Relationships
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

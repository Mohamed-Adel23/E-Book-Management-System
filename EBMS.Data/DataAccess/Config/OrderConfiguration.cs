using EBMS.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace EBMS.Data.DataAccess.Config
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd();
            builder.Property(x => x.ShipAddress)
                   .HasColumnType("NVARCHAR")
                   .HasMaxLength(255)
                   .IsRequired();
            builder.Property(x => x.Status)
                   .HasColumnType("VARCHAR")
                   .HasMaxLength(50)
                   .IsRequired(false);
            builder.Property(x => x.PostalCode)
                   .HasColumnType("VARCHAR")
                   .HasMaxLength(100)
                   .IsRequired();
            builder.Property(x => x.PhoneNumber)
                   .HasColumnType("VARCHAR")
                   .HasMaxLength(100)
                   .IsRequired();
            builder.Property(x => x.Created_at)
                   .HasColumnType("DATETIME")
                   .HasDefaultValue(DateTime.UtcNow);
            builder.Property(x => x.Updated_at)
                   .HasColumnType("DATETIME")
                   .IsRequired(false);
        }
    }
}

using EBMS.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace EBMS.Data.DataAccess.Config
{
    public class PaymentInfoConfiguration : IEntityTypeConfiguration<PaymentInfo>
    {
        public void Configure(EntityTypeBuilder<PaymentInfo> builder)
        {
            builder.ToTable("PaymentInfo");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(x => x.PayerId)
                   .HasColumnType("NVARCHAR")
                   .HasMaxLength(255)
                   .IsRequired();
            builder.Property(x => x.PaymentId)
                   .HasColumnType("NVARCHAR")
                   .HasMaxLength(255)
                   .IsRequired();
            builder.Property(x => x.Method)
                   .HasColumnType("VARCHAR")
                   .HasMaxLength(100)
                   .IsRequired();
            builder.Property(x => x.Currency)
                   .HasColumnType("NVARCHAR")
                   .HasMaxLength(50)
                   .IsRequired();
            builder.Property(x => x.Amount)
                   .HasColumnType("DECIMAL")
                   .HasPrecision(7, 2)
                   .IsRequired();
            builder.Property(x => x.Status)
                   .HasColumnType("VARCHAR")
                   .HasMaxLength(50)
                   .IsRequired(false);
            builder.Property(x => x.Paid_at)
                   .HasColumnType("DATETIME")
                   .IsRequired();

            // 1 Order => 1 Payment
            builder.HasOne(x => x.Order)
                   .WithOne(x => x.PaymentInfo)
                   .HasForeignKey<PaymentInfo>(x => x.OrderId)
                   .IsRequired();
        }
    }
}

using EBMS.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace EBMS.Data.DataAccess.Config
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd();
            builder.Property(x => x.Rate)
                   .HasColumnType("TINYINT")
                   .IsRequired();
            builder.Property(x => x.Comment)
                   .HasColumnType("NVARCHAR")
                   .HasMaxLength(255)
                   .IsRequired(false);
            builder.Property(x => x.Created_at)
                   .HasColumnType("DATETIME")
                   .HasDefaultValue(DateTime.UtcNow);
            builder.Property(x => x.Updated_at)
                   .HasColumnType("DATETIME")
                   .IsRequired(false);
        }
    }
}

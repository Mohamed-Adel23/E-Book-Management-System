using EBMS.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace EBMS.Data.DataAccess.Config
{
    public class PaidDownloadConfiguration : IEntityTypeConfiguration<BookDownload>
    {
        public void Configure(EntityTypeBuilder<BookDownload> builder)
        {
            builder.ToTable("BookDownloads");
            builder.HasKey(x => new { x.BookId, x.BookUserId });
            builder.Property(x => x.CurrentPrice)
                   .HasColumnType("DECIMAL")
                   .HasPrecision(7, 2)
                   .IsRequired();
            builder.Property(x => x.Status)
                   .HasColumnType("BIT")
                   .HasDefaultValue(0);
            builder.Property(x => x.Downloaded_at)
                   .HasColumnType("DATETIME")
                   .IsRequired();
        }
    }
}

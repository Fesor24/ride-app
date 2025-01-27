using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ridely.Domain.Rides;

namespace Ridely.Infrastructure.Configurations;
internal sealed class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.ToTable(nameof(Chat), ApplicationDbContext.Rides);

        builder.Property(chat => chat.Message)
            .HasMaxLength(200);
    }
}

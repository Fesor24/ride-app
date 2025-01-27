using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ridely.Infrastructure.Outbox;

namespace Ridely.Infrastructure.Configurations;
internal sealed class OutboxMessagesConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages", ApplicationDbContext.Common);
        builder.HasKey(message => message.Id);
        builder.Property(message => message.Content)
            .HasColumnType("jsonb");
    }
}

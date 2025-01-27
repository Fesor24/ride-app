using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ridely.Domain.Call;

namespace Ridely.Infrastructure.Configurations
{
    internal sealed class CallLogConfiguration : IEntityTypeConfiguration<CallLog>
    {
        public void Configure(EntityTypeBuilder<CallLog> builder)
        {
            builder.ToTable(nameof(CallLog), ApplicationDbContext.Rides);
            builder.HasKey(log => log.Id);
        }
    }
}

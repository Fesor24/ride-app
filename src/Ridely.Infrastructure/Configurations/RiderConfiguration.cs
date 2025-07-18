﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ridely.Domain.Riders;

namespace Ridely.Infrastructure.Configurations;
internal sealed class RiderConfiguration : IEntityTypeConfiguration<Rider>
{
    public void Configure(EntityTypeBuilder<Rider> builder)
    {
        builder.ToTable(nameof(Rider), ApplicationDbContext.Rider);

        builder.Property(rider => rider.FirstName)
            .HasMaxLength(60);

        builder.Property(rider => rider.LastName)
            .HasMaxLength(60);

        builder.HasIndex(rider => rider.PhoneNo)
            .IsUnique();

        builder.HasIndex(rider => rider.Email)
            .IsUnique();

        builder.Property(rider => rider.PhoneNo)
           .HasMaxLength(15);

        builder.Property(rider => rider.Email)
            .HasMaxLength(70);

        builder.HasIndex(rider => rider.ReferralCode)
            .IsUnique();

        builder.HasQueryFilter(rider => !rider.IsDeleted);
    }
}

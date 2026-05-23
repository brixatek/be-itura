using Itura.Corporate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Corporate.Infrastructure.Persistence.Configurations;

internal sealed class CorporateAccountConfiguration : IEntityTypeConfiguration<CorporateAccount>
{
    public void Configure(EntityTypeBuilder<CorporateAccount> builder)
    {
        builder.ToTable("corporate_accounts");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.CompanyName).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Industry).HasMaxLength(100);
        builder.Property(e => e.LogoUrl).HasMaxLength(500);
        builder.Property(e => e.Website).HasMaxLength(300);
        builder.Property(e => e.SubscriptionTier).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(e => e.AdminUserId).IsUnique();
    }
}

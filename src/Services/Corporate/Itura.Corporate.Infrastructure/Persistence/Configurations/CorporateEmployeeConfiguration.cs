using Itura.Corporate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Corporate.Infrastructure.Persistence.Configurations;

internal sealed class CorporateEmployeeConfiguration : IEntityTypeConfiguration<CorporateEmployee>
{
    public void Configure(EntityTypeBuilder<CorporateEmployee> builder)
    {
        builder.ToTable("corporate_employees");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Role).HasConversion<string>().HasMaxLength(20);
        builder.HasQueryFilter(e => !e.IsDeleted);
        builder.HasIndex(e => e.CorporateAccountId);
        builder.HasIndex(e => new { e.CorporateAccountId, e.UserId }).IsUnique();
    }
}

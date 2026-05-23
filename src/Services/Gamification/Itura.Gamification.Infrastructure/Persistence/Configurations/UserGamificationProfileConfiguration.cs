using Itura.Gamification.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Gamification.Infrastructure.Persistence.Configurations;

internal sealed class UserGamificationProfileConfiguration : IEntityTypeConfiguration<UserGamificationProfile>
{
    public void Configure(EntityTypeBuilder<UserGamificationProfile> builder)
    {
        builder.ToTable("user_gamification_profiles");
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.UserId).IsUnique();
        builder.HasIndex(e => e.TotalPoints);
    }
}

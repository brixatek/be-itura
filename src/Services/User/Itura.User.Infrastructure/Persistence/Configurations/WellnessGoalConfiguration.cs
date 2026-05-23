using Itura.User.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.User.Infrastructure.Persistence.Configurations;

public sealed class WellnessGoalConfiguration : IEntityTypeConfiguration<WellnessGoal>
{
    public void Configure(EntityTypeBuilder<WellnessGoal> builder)
    {
        builder.ToTable("wellness_goals");

        builder.HasKey(g => g.Id);
        builder.Property(g => g.Id).HasColumnName("id");

        builder.Property(g => g.UserProfileId).HasColumnName("user_profile_id").IsRequired();
        builder.Property(g => g.Goal).HasColumnName("goal").HasMaxLength(100).IsRequired();
        builder.Property(g => g.Priority).HasColumnName("priority").IsRequired();
    }
}

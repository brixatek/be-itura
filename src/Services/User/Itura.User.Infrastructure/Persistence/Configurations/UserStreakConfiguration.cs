using Itura.User.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.User.Infrastructure.Persistence.Configurations;

internal sealed class UserStreakConfiguration : IEntityTypeConfiguration<UserStreak>
{
    public void Configure(EntityTypeBuilder<UserStreak> builder)
    {
        builder.ToTable("user_streaks");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(s => s.UserProfileId).HasColumnName("user_profile_id").IsRequired();
        builder.Property(s => s.StreakType).HasColumnName("streak_type").HasMaxLength(50).IsRequired();
        builder.Property(s => s.CurrentStreak).HasColumnName("current_streak").IsRequired();
        builder.Property(s => s.LongestStreak).HasColumnName("longest_streak").IsRequired();
        builder.Property(s => s.LastActivityDate).HasColumnName("last_activity_date").IsRequired();
        builder.Property(s => s.CreatedAt).HasColumnName("created_at");
        builder.Property(s => s.UpdatedAt).HasColumnName("updated_at");
        builder.Property(s => s.DeletedAt).HasColumnName("deleted_at");

        builder.HasIndex(s => new { s.UserProfileId, s.StreakType })
            .IsUnique()
            .HasDatabaseName("ix_user_streaks_user_type");
        builder.HasIndex(s => s.LastActivityDate).HasDatabaseName("ix_user_streaks_last_activity_date");
    }
}

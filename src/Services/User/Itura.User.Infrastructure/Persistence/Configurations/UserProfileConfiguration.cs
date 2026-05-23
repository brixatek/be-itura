using Itura.User.Domain.Entities;
using Itura.User.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.User.Infrastructure.Persistence.Configurations;

public sealed class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("user_profiles");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id").ValueGeneratedNever();

        builder.Property(p => p.Email).HasColumnName("email").HasMaxLength(256).IsRequired();
        builder.Property(p => p.FullName).HasColumnName("full_name").HasMaxLength(100).IsRequired();
        builder.Property(p => p.Timezone).HasColumnName("timezone").HasMaxLength(50).IsRequired();
        builder.Property(p => p.AvatarUrl).HasColumnName("avatar_url").HasMaxLength(500);
        builder.Property(p => p.Bio).HasColumnName("bio").HasMaxLength(500);
        builder.Property(p => p.DateOfBirth).HasColumnName("date_of_birth");
        builder.Property(p => p.Gender).HasColumnName("gender")
            .HasConversion(g => g.HasValue ? g.Value.ToString() : null, s => s != null ? Enum.Parse<Gender>(s) : null)
            .HasMaxLength(20);
        builder.Property(p => p.OnboardingCompleted).HasColumnName("onboarding_completed").HasDefaultValue(false);

        builder.Property(p => p.TotalXp).HasColumnName("total_xp").HasDefaultValue(0);
        builder.Property(p => p.WellnessLevel).HasColumnName("wellness_level").HasDefaultValue(1);
        builder.Property(p => p.IsAnonymized).HasColumnName("is_anonymized").HasDefaultValue(false);
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");
        builder.Property(p => p.DeletedAt).HasColumnName("deleted_at");

        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.Property(p => p.Language).HasColumnName("language").HasMaxLength(10).HasDefaultValue("en");
        builder.Property(p => p.EmailNotifications).HasColumnName("email_notifications").HasDefaultValue(true);
        builder.Property(p => p.PushNotifications).HasColumnName("push_notifications").HasDefaultValue(true);
        builder.Property(p => p.WeeklyDigest).HasColumnName("weekly_digest").HasDefaultValue(true);
        builder.Property(p => p.Theme).HasColumnName("theme").HasMaxLength(10).HasDefaultValue("system");

        builder.HasIndex(p => p.Email).IsUnique();

        builder.HasMany(p => p.WellnessGoals)
            .WithOne()
            .HasForeignKey(g => g.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore("DomainEvents");
    }
}

using Itura.Coach.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Coach.Infrastructure.Persistence.Configurations;

internal sealed class CoachAvailabilityConfiguration : IEntityTypeConfiguration<CoachAvailability>
{
    public void Configure(EntityTypeBuilder<CoachAvailability> builder)
    {
        builder.ToTable("coach_availability");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(a => a.CoachUserId).HasColumnName("coach_user_id").IsRequired();
        builder.Property(a => a.DayOfWeek).HasColumnName("day_of_week").HasConversion<int>().IsRequired();
        builder.Property(a => a.StartTime).HasColumnName("start_time").IsRequired();
        builder.Property(a => a.EndTime).HasColumnName("end_time").IsRequired();
        builder.Property(a => a.SlotDurationMinutes).HasColumnName("slot_duration_minutes").HasDefaultValue(60);
        builder.Property(a => a.Timezone).HasColumnName("timezone").HasMaxLength(50).HasDefaultValue("UTC");
        builder.Property(a => a.CreatedAt).HasColumnName("created_at");
        builder.Property(a => a.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(a => a.CoachUserId).HasDatabaseName("ix_coach_availability_coach_user_id");
    }
}

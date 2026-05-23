using Itura.Coach.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Coach.Infrastructure.Persistence.Configurations;

internal sealed class CoachBlockedTimeConfiguration : IEntityTypeConfiguration<CoachBlockedTime>
{
    public void Configure(EntityTypeBuilder<CoachBlockedTime> builder)
    {
        builder.ToTable("coach_blocked_times");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(b => b.CoachUserId).HasColumnName("coach_user_id").IsRequired();
        builder.Property(b => b.StartUtc).HasColumnName("start_utc").IsRequired();
        builder.Property(b => b.EndUtc).HasColumnName("end_utc").IsRequired();
        builder.Property(b => b.Reason).HasColumnName("reason").HasMaxLength(200);
        builder.Property(b => b.CreatedAt).HasColumnName("created_at");
        builder.Property(b => b.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(b => new { b.CoachUserId, b.StartUtc }).HasDatabaseName("ix_coach_blocked_times_coach_start");
    }
}

using Itura.User.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Itura.User.Infrastructure.Persistence.Configurations;

internal sealed class WellnessAssessmentConfiguration : IEntityTypeConfiguration<WellnessAssessment>
{
    public void Configure(EntityTypeBuilder<WellnessAssessment> builder)
    {
        builder.ToTable("wellness_assessments");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(a => a.UserProfileId).HasColumnName("user_profile_id").IsRequired();
        builder.Property(a => a.CompositeScore).HasColumnName("composite_score").IsRequired();
        builder.Property(a => a.RiskLevel).HasColumnName("risk_level").HasMaxLength(20).IsRequired();
        var comparer = new ValueComparer<Dictionary<string, int>>(
            (a, b) => JsonSerializer.Serialize(a, (JsonSerializerOptions?)null) == JsonSerializer.Serialize(b, (JsonSerializerOptions?)null),
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null).GetHashCode(),
            v => new Dictionary<string, int>(v));
        builder.Property(a => a.Answers).HasColumnName("answers").HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, int>>(v, (JsonSerializerOptions?)null) ?? new(),
                comparer);
        builder.Property(a => a.CreatedAt).HasColumnName("created_at");
        builder.Property(a => a.UpdatedAt).HasColumnName("updated_at");
        builder.Property(a => a.DeletedAt).HasColumnName("deleted_at");

        builder.HasIndex(a => a.UserProfileId).HasDatabaseName("ix_wellness_assessments_user_profile_id");
    }
}

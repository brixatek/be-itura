using Itura.Coach.Domain.Entities;
using Itura.Coach.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Coach.Infrastructure.Persistence.Configurations;

internal sealed class CoachConfiguration : IEntityTypeConfiguration<CoachProfile>
{
    public void Configure(EntityTypeBuilder<CoachProfile> builder)
    {
        builder.ToTable("coaches");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(c => c.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(c => c.Email).HasColumnName("email").HasMaxLength(255).IsRequired().HasDefaultValue(string.Empty);
        builder.Property(c => c.DisplayName).HasColumnName("display_name").HasMaxLength(100).IsRequired();
        builder.Property(c => c.Bio).HasColumnName("bio").HasMaxLength(2000).IsRequired();
        builder.Property(c => c.Specializations).HasColumnName("specializations").HasColumnType("jsonb").IsRequired();
        builder.Property(c => c.Languages).HasColumnName("languages").HasColumnType("jsonb").IsRequired();
        builder.Property(c => c.HourlyRate).HasColumnName("hourly_rate").HasPrecision(18, 2).IsRequired();
        builder.Property(c => c.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
        builder.Property(c => c.ProfileImageUrl).HasColumnName("profile_image_url").HasMaxLength(500);
        builder.Property(c => c.YearsOfExperience).HasColumnName("years_of_experience").IsRequired();
        builder.Property(c => c.IsActive).HasColumnName("is_active").IsRequired();
        builder.Property(c => c.AverageRating).HasColumnName("average_rating");
        builder.Property(c => c.TotalReviews).HasColumnName("total_reviews").IsRequired();
        builder.Property(c => c.VerificationStatus)
            .HasColumnName("verification_status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(VerificationStatus.Pending)
            .IsRequired();
        builder.Property(c => c.VerifiedAt).HasColumnName("verified_at");
        builder.Property(c => c.VerifiedBy).HasColumnName("verified_by");
        builder.Property(c => c.RejectionReason).HasColumnName("rejection_reason").HasMaxLength(500);
        builder.Property(c => c.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
        builder.Property(c => c.DeletedAt).HasColumnName("deleted_at");

        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.HasIndex(c => c.UserId).IsUnique().HasDatabaseName("ix_coaches_user_id");
        builder.HasIndex(c => c.IsActive).HasDatabaseName("ix_coaches_is_active");
        builder.HasIndex(c => c.VerificationStatus).HasDatabaseName("ix_coaches_verification_status");
        builder.HasIndex(c => c.AverageRating).HasDatabaseName("ix_coaches_average_rating");
    }
}

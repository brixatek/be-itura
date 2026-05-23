using Itura.AI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.AI.Infrastructure.Persistence.Configurations;

internal sealed class UserRecommendationConfiguration : IEntityTypeConfiguration<UserRecommendation>
{
    public void Configure(EntityTypeBuilder<UserRecommendation> builder)
    {
        builder.ToTable("user_recommendations");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.RecommendationType).HasMaxLength(50).IsRequired();
        builder.Property(r => r.EntityType).HasMaxLength(50).IsRequired();
        builder.Property(r => r.Reason).HasMaxLength(500);
        builder.HasIndex(r => new { r.UserId, r.RecommendationType, r.IsActive });
        builder.HasIndex(r => r.ExpiresAt);
    }
}

using Itura.User.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.User.Infrastructure.Persistence.Configurations;

internal sealed class BadgeEarnedConfiguration : IEntityTypeConfiguration<BadgeEarned>
{
    public void Configure(EntityTypeBuilder<BadgeEarned> builder)
    {
        builder.ToTable("badges_earned");

        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(b => b.UserProfileId).HasColumnName("user_profile_id").IsRequired();
        builder.Property(b => b.BadgeDefinitionId).HasColumnName("badge_definition_id").IsRequired();
        builder.Property(b => b.CreatedAt).HasColumnName("created_at");
        builder.Property(b => b.UpdatedAt).HasColumnName("updated_at");
        builder.Property(b => b.DeletedAt).HasColumnName("deleted_at");

        builder.HasOne(b => b.BadgeDefinition).WithMany()
            .HasForeignKey(b => b.BadgeDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => new { b.UserProfileId, b.BadgeDefinitionId })
            .IsUnique()
            .HasDatabaseName("ix_badges_earned_user_badge");
    }
}

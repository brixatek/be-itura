using Itura.Community.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Community.Infrastructure.Persistence.Configurations;

internal sealed class CommunityPostConfiguration : IEntityTypeConfiguration<CommunityPost>
{
    public void Configure(EntityTypeBuilder<CommunityPost> builder)
    {
        builder.ToTable("community_posts");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).HasMaxLength(300);
        builder.Property(e => e.Body).IsRequired().HasMaxLength(5000);
        builder.Property(e => e.PostType).HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.Tags).HasColumnType("text[]");
        builder.Property(e => e.IsAnonymous).HasDefaultValue(false);
        builder.Property(e => e.ReactionCount).HasColumnName("reaction_count").HasDefaultValue(0);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.HasIndex(e => e.AuthorUserId);
        builder.HasIndex(e => e.PostType);
        builder.HasIndex(e => e.CreatedAt);
    }
}

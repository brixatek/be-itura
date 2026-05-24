using Itura.Community.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Community.Infrastructure.Persistence.Configurations;

internal sealed class PostReactionConfiguration : IEntityTypeConfiguration<PostReaction>
{
    public void Configure(EntityTypeBuilder<PostReaction> builder)
    {
        builder.ToTable("post_reactions");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(r => r.PostId).HasColumnName("post_id").IsRequired();
        builder.Property(r => r.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(r => r.Emoji).HasColumnName("emoji").HasMaxLength(10).IsRequired();
        builder.Property(r => r.CreatedAt).HasColumnName("created_at");
        builder.Property(r => r.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(r => r.PostId).HasDatabaseName("ix_post_reactions_post_id");
        builder.HasIndex(r => new { r.PostId, r.UserId, r.Emoji })
            .IsUnique()
            .HasDatabaseName("ix_post_reactions_unique");
    }
}

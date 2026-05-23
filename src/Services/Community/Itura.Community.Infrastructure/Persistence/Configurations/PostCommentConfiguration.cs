using Itura.Community.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Community.Infrastructure.Persistence.Configurations;

internal sealed class PostCommentConfiguration : IEntityTypeConfiguration<PostComment>
{
    public void Configure(EntityTypeBuilder<PostComment> builder)
    {
        builder.ToTable("post_comments");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Body).IsRequired().HasMaxLength(2000);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.HasIndex(e => e.PostId);
        builder.HasIndex(e => e.AuthorUserId);
        builder.HasIndex(e => e.CreatedAt);
    }
}

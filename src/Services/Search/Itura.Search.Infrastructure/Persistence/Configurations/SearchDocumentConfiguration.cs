using Itura.Search.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Search.Infrastructure.Persistence.Configurations;

internal sealed class SearchDocumentConfiguration : IEntityTypeConfiguration<SearchDocument>
{
    public void Configure(EntityTypeBuilder<SearchDocument> builder)
    {
        builder.ToTable("search_documents");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.EntityType).HasMaxLength(50).IsRequired();
        builder.Property(d => d.Title).HasMaxLength(500).IsRequired();
        builder.Property(d => d.Description).HasMaxLength(2000);
        builder.Property(d => d.Tags).HasColumnType("text[]");
        builder.HasIndex(d => new { d.EntityType, d.EntityId }).IsUnique();
        builder.HasIndex(d => d.IsActive);
    }
}

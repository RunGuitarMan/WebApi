using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApi.Domain;

namespace WebApi.Persistence.Configuration;

public class TestEntityConfiguration : IEntityTypeConfiguration<TestEntity>
{
    public void Configure(EntityTypeBuilder<TestEntity> builder)
    {
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Content);
        
        builder.HasIndex(entity => entity.Id)
            .IsUnique();
    }
}
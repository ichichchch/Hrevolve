using Hrevolve.Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Hrevolve.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");
        
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Name)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(t => t.Code)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(t => t.Domain)
            .HasMaxLength(200);
        
        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.Property(t => t.Plan)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.Property(t => t.ConnectionString)
            .HasMaxLength(1000);
        
        builder.Property(t => t.EncryptionKey)
            .HasMaxLength(500);
        
        // 将Settings序列化为JSON存储
        builder.Property(t => t.Settings)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<TenantSettings>(v, (JsonSerializerOptions?)null) ?? new TenantSettings())
            .HasMaxLength(2000);
        
        builder.HasIndex(t => t.Code)
            .IsUnique();
        
        builder.HasIndex(t => t.Domain)
            .IsUnique();
        
        builder.Ignore(t => t.DomainEvents);
    }
}

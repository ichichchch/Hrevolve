using Hrevolve.Domain.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrevolve.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.UserName)
            .HasMaxLength(100);
        
        builder.Property(a => a.Action)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(a => a.EntityType)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(a => a.EntityId)
            .HasMaxLength(100);
        
        builder.Property(a => a.AffectedColumns)
            .HasMaxLength(2000);
        
        builder.Property(a => a.IpAddress)
            .HasMaxLength(50);
        
        builder.Property(a => a.UserAgent)
            .HasMaxLength(500);
        
        builder.Property(a => a.RequestPath)
            .HasMaxLength(500);
        
        builder.Property(a => a.TraceId)
            .HasMaxLength(100);
        
        builder.Property(a => a.CorrelationId)
            .HasMaxLength(100);
        
        // 索引 - 支持审计查询
        builder.HasIndex(a => new { a.TenantId, a.Timestamp });
        builder.HasIndex(a => new { a.TenantId, a.UserId, a.Timestamp });
        builder.HasIndex(a => new { a.TenantId, a.EntityType, a.EntityId });
        builder.HasIndex(a => a.TraceId);
        
        builder.Ignore(a => a.DomainEvents);
    }
}

using Hrevolve.Domain.Organizations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrevolve.Infrastructure.Persistence.Configurations;

public class OrganizationUnitConfiguration : IEntityTypeConfiguration<OrganizationUnit>
{
    public void Configure(EntityTypeBuilder<OrganizationUnit> builder)
    {
        builder.ToTable("OrganizationUnits");
        
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.Name)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(o => o.Code)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(o => o.Description)
            .HasMaxLength(500);
        
        builder.Property(o => o.Path)
            .HasMaxLength(1000)
            .IsRequired();
        
        builder.Property(o => o.Type)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        // 索引
        builder.HasIndex(o => new { o.TenantId, o.Code })
            .IsUnique();
        
        builder.HasIndex(o => new { o.TenantId, o.ParentId });
        builder.HasIndex(o => new { o.TenantId, o.Path });
        
        // 自引用关系 - 父级组织
        builder.HasOne(o => o.Parent)
            .WithMany(o => o.Children)
            .HasForeignKey(o => o.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Ignore(o => o.DomainEvents);
    }
}

public class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("Positions");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(p => p.Code)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(p => p.Description)
            .HasMaxLength(500);
        
        builder.Property(p => p.Sequence)
            .HasMaxLength(50);
        
        builder.Property(p => p.Level)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.Property(p => p.SalaryRangeMin)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.SalaryRangeMax)
            .HasPrecision(18, 2);
        
        // 索引
        builder.HasIndex(p => new { p.TenantId, p.Code })
            .IsUnique();
        
        builder.HasIndex(p => new { p.TenantId, p.OrganizationUnitId });
        
        builder.HasOne(p => p.OrganizationUnit)
            .WithMany()
            .HasForeignKey(p => p.OrganizationUnitId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Ignore(p => p.DomainEvents);
    }
}

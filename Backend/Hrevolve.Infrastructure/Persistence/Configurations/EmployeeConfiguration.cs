using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrevolve.Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.EmployeeNumber)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(e => e.FirstName)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(e => e.LastName)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(e => e.EnglishName)
            .HasMaxLength(100);
        
        builder.Property(e => e.IdCardNumber)
            .HasMaxLength(500); // 加密后长度
        
        builder.Property(e => e.Email)
            .HasMaxLength(256);
        
        builder.Property(e => e.Phone)
            .HasMaxLength(20);
        
        builder.Property(e => e.PersonalEmail)
            .HasMaxLength(256);
        
        builder.Property(e => e.Address)
            .HasMaxLength(500);
        
        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.Property(e => e.EmploymentType)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.Property(e => e.Gender)
            .HasConversion<string>()
            .HasMaxLength(10);
        
        // 索引
        builder.HasIndex(e => new { e.TenantId, e.EmployeeNumber })
            .IsUnique();
        
        builder.HasIndex(e => new { e.TenantId, e.Email });
        builder.HasIndex(e => new { e.TenantId, e.Status });
        
        // 自引用关系 - 直属上级
        builder.HasOne(e => e.DirectManager)
            .WithMany()
            .HasForeignKey(e => e.DirectManagerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // 忽略领域事件
        builder.Ignore(e => e.DomainEvents);
    }
}

public class JobHistoryConfiguration : IEntityTypeConfiguration<JobHistory>
{
    public void Configure(EntityTypeBuilder<JobHistory> builder)
    {
        builder.ToTable("JobHistories");
        
        builder.HasKey(j => j.Id);
        
        builder.Property(j => j.BaseSalary)
            .HasPrecision(18, 2);
        
        builder.Property(j => j.Grade)
            .HasMaxLength(50);
        
        builder.Property(j => j.ChangeType)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.Property(j => j.ChangeReason)
            .HasMaxLength(500);
        
        builder.Property(j => j.CorrectionStatus)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        // 索引 - 支持时点查询
        builder.HasIndex(j => new { j.TenantId, j.EmployeeId, j.EffectiveStartDate, j.EffectiveEndDate });
        builder.HasIndex(j => new { j.TenantId, j.EmployeeId, j.CorrectionStatus });
        
        builder.HasOne(j => j.Employee)
            .WithMany(e => e.JobHistories)
            .HasForeignKey(j => j.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Ignore(j => j.DomainEvents);
    }
}

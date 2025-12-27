using Hrevolve.Domain.Payroll;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrevolve.Infrastructure.Persistence.Configurations;

public class PayrollPeriodConfiguration : IEntityTypeConfiguration<PayrollPeriod>
{
    public void Configure(EntityTypeBuilder<PayrollPeriod> builder)
    {
        builder.ToTable("PayrollPeriods");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.HasIndex(p => new { p.TenantId, p.Year, p.Month })
            .IsUnique();
        
        builder.Ignore(p => p.DomainEvents);
    }
}

public class PayrollItemConfiguration : IEntityTypeConfiguration<PayrollItem>
{
    public void Configure(EntityTypeBuilder<PayrollItem> builder)
    {
        builder.ToTable("PayrollItems");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(p => p.Code)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(p => p.Description)
            .HasMaxLength(500);
        
        builder.Property(p => p.Type)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.Property(p => p.CalculationType)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.Property(p => p.FixedAmount)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.Formula)
            .HasMaxLength(2000);
        
        builder.HasIndex(p => new { p.TenantId, p.Code })
            .IsUnique();
        
        builder.Ignore(p => p.DomainEvents);
    }
}

public class PayrollRecordConfiguration : IEntityTypeConfiguration<PayrollRecord>
{
    public void Configure(EntityTypeBuilder<PayrollRecord> builder)
    {
        builder.ToTable("PayrollRecords");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.BaseSalary)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.GrossSalary)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.TotalDeductions)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.NetSalary)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.IncomeTax)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.SocialInsuranceEmployee)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.SocialInsuranceEmployer)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.HousingFundEmployee)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.HousingFundEmployer)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.HasIndex(p => new { p.TenantId, p.EmployeeId, p.PayrollPeriodId })
            .IsUnique();
        
        builder.HasOne(p => p.PayrollPeriod)
            .WithMany()
            .HasForeignKey(p => p.PayrollPeriodId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(p => p.Details)
            .WithOne()
            .HasForeignKey(d => d.PayrollRecordId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Ignore(p => p.DomainEvents);
    }
}

public class PayrollDetailConfiguration : IEntityTypeConfiguration<PayrollDetail>
{
    public void Configure(EntityTypeBuilder<PayrollDetail> builder)
    {
        builder.ToTable("PayrollDetails");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.ItemName)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(p => p.ItemType)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.Property(p => p.Amount)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.Remarks)
            .HasMaxLength(500);
    }
}

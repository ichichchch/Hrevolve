using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrevolve.Infrastructure.Persistence.Configurations;

public class LeaveTypeConfiguration : IEntityTypeConfiguration<LeaveType>
{
    public void Configure(EntityTypeBuilder<LeaveType> builder)
    {
        builder.ToTable("LeaveTypes");
        
        builder.HasKey(l => l.Id);
        
        builder.Property(l => l.Name)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(l => l.Code)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(l => l.Description)
            .HasMaxLength(500);
        
        builder.Property(l => l.MinUnit)
            .HasPrecision(5, 2);
        
        builder.Property(l => l.Color)
            .HasMaxLength(20);
        
        builder.HasIndex(l => new { l.TenantId, l.Code })
            .IsUnique();
        
        builder.Ignore(l => l.DomainEvents);
    }
}

public class LeavePolicyConfiguration : IEntityTypeConfiguration<LeavePolicy>
{
    public void Configure(EntityTypeBuilder<LeavePolicy> builder)
    {
        builder.ToTable("LeavePolicies");
        
        builder.HasKey(l => l.Id);
        
        builder.Property(l => l.Name)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(l => l.BaseQuota)
            .HasPrecision(5, 2);
        
        builder.Property(l => l.MaxCarryOverDays)
            .HasPrecision(5, 2);
        
        builder.Property(l => l.SeniorityRules)
            .HasMaxLength(2000);
        
        builder.Property(l => l.GradeRules)
            .HasMaxLength(2000);
        
        builder.Property(l => l.AccrualPeriod)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.HasOne(l => l.LeaveType)
            .WithMany()
            .HasForeignKey(l => l.LeaveTypeId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Ignore(l => l.DomainEvents);
    }
}

public class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
{
    public void Configure(EntityTypeBuilder<LeaveRequest> builder)
    {
        builder.ToTable("LeaveRequests");
        
        builder.HasKey(l => l.Id);
        
        builder.Property(l => l.StartDayPart)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.Property(l => l.EndDayPart)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.Property(l => l.TotalDays)
            .HasPrecision(5, 2);
        
        builder.Property(l => l.Reason)
            .HasMaxLength(1000)
            .IsRequired();
        
        builder.Property(l => l.Attachments)
            .HasMaxLength(2000);
        
        builder.Property(l => l.Status)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.Property(l => l.CancelReason)
            .HasMaxLength(500);
        
        builder.HasIndex(l => new { l.TenantId, l.EmployeeId, l.Status });
        builder.HasIndex(l => new { l.TenantId, l.StartDate, l.EndDate });
        
        builder.HasOne(l => l.LeaveType)
            .WithMany()
            .HasForeignKey(l => l.LeaveTypeId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(l => l.Approvals)
            .WithOne()
            .HasForeignKey(a => a.LeaveRequestId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Ignore(l => l.DomainEvents);
    }
}

public class LeaveApprovalConfiguration : IEntityTypeConfiguration<LeaveApproval>
{
    public void Configure(EntityTypeBuilder<LeaveApproval> builder)
    {
        builder.ToTable("LeaveApprovals");
        
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Action)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.Property(a => a.Comments)
            .HasMaxLength(500);
    }
}

public class LeaveBalanceConfiguration : IEntityTypeConfiguration<LeaveBalance>
{
    public void Configure(EntityTypeBuilder<LeaveBalance> builder)
    {
        builder.ToTable("LeaveBalances");
        
        builder.HasKey(l => l.Id);
        
        builder.Property(l => l.Entitlement)
            .HasPrecision(5, 2);
        
        builder.Property(l => l.CarriedOver)
            .HasPrecision(5, 2);
        
        builder.Property(l => l.Used)
            .HasPrecision(5, 2);
        
        builder.Property(l => l.Pending)
            .HasPrecision(5, 2);
        
        builder.HasIndex(l => new { l.TenantId, l.EmployeeId, l.LeaveTypeId, l.Year })
            .IsUnique();
        
        builder.Ignore(l => l.DomainEvents);
    }
}

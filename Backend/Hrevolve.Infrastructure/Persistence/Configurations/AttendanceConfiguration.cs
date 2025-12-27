using Hrevolve.Domain.Attendance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrevolve.Infrastructure.Persistence.Configurations;

public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
{
    public void Configure(EntityTypeBuilder<Shift> builder)
    {
        builder.ToTable("Shifts");
        
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Name)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(s => s.Code)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(s => s.Description)
            .HasMaxLength(500);
        
        builder.Property(s => s.StandardHours)
            .HasPrecision(5, 2);
        
        builder.HasIndex(s => new { s.TenantId, s.Code })
            .IsUnique();
        
        builder.Ignore(s => s.DomainEvents);
    }
}

public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.ToTable("Schedules");
        
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Remarks)
            .HasMaxLength(500);
        
        builder.HasIndex(s => new { s.TenantId, s.EmployeeId, s.ScheduleDate })
            .IsUnique();
        
        builder.HasOne(s => s.Shift)
            .WithMany()
            .HasForeignKey(s => s.ShiftId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Ignore(s => s.DomainEvents);
    }
}

public class AttendanceRecordConfiguration : IEntityTypeConfiguration<AttendanceRecord>
{
    public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
    {
        builder.ToTable("AttendanceRecords");
        
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.CheckInMethod)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.Property(a => a.CheckOutMethod)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.Property(a => a.CheckInLocation)
            .HasMaxLength(200);
        
        builder.Property(a => a.CheckOutLocation)
            .HasMaxLength(200);
        
        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.Property(a => a.ActualHours)
            .HasPrecision(5, 2);
        
        builder.Property(a => a.OvertimeHours)
            .HasPrecision(5, 2);
        
        builder.Property(a => a.Remarks)
            .HasMaxLength(500);
        
        builder.HasIndex(a => new { a.TenantId, a.EmployeeId, a.AttendanceDate })
            .IsUnique();
        
        builder.HasIndex(a => new { a.TenantId, a.AttendanceDate, a.Status });
        
        builder.HasOne(a => a.Schedule)
            .WithMany()
            .HasForeignKey(a => a.ScheduleId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.Ignore(a => a.DomainEvents);
    }
}

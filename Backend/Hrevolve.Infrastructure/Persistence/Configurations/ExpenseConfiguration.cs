using Hrevolve.Domain.Expense;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrevolve.Infrastructure.Persistence.Configurations;

public class ExpenseRequestConfiguration : IEntityTypeConfiguration<ExpenseRequest>
{
    public void Configure(EntityTypeBuilder<ExpenseRequest> builder)
    {
        builder.ToTable("ExpenseRequests");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Title)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(e => e.Description)
            .HasMaxLength(1000);
        
        builder.Property(e => e.TotalAmount)
            .HasPrecision(18, 2);
        
        builder.Property(e => e.Currency)
            .HasMaxLength(10)
            .IsRequired();
        
        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.HasIndex(e => new { e.TenantId, e.EmployeeId, e.Status });
        
        builder.HasMany(e => e.Items)
            .WithOne()
            .HasForeignKey(i => i.ExpenseRequestId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(e => e.Approvals)
            .WithOne()
            .HasForeignKey(a => a.ExpenseRequestId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Ignore(e => e.DomainEvents);
    }
}

public class ExpenseItemConfiguration : IEntityTypeConfiguration<ExpenseItem>
{
    public void Configure(EntityTypeBuilder<ExpenseItem> builder)
    {
        builder.ToTable("ExpenseItems");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Category)
            .HasConversion<string>()
            .HasMaxLength(30);
        
        builder.Property(e => e.Amount)
            .HasPrecision(18, 2);
        
        builder.Property(e => e.Description)
            .HasMaxLength(500);
        
        builder.Property(e => e.ReceiptUrl)
            .HasMaxLength(500);
    }
}

public class ExpenseApprovalConfiguration : IEntityTypeConfiguration<ExpenseApproval>
{
    public void Configure(EntityTypeBuilder<ExpenseApproval> builder)
    {
        builder.ToTable("ExpenseApprovals");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Action)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.Property(e => e.Comments)
            .HasMaxLength(500);
    }
}

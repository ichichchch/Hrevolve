using Hrevolve.Domain.Audit;
using Hrevolve.Domain.Expense;

namespace Hrevolve.Infrastructure.Persistence;

/// <summary>
/// 主数据库上下文
/// </summary>
public class HrevolveDbContext : DbContext
{
    private readonly ITenantContextAccessor _tenantContextAccessor;
    private readonly ICurrentUserAccessor _currentUserAccessor;
    
    public HrevolveDbContext(
        DbContextOptions<HrevolveDbContext> options,
        ITenantContextAccessor tenantContextAccessor,
        ICurrentUserAccessor currentUserAccessor)
        : base(options)
    {
        _tenantContextAccessor = tenantContextAccessor;
        _currentUserAccessor = currentUserAccessor;
    }
    
    // 租户
    public DbSet<Tenant> Tenants => Set<Tenant>();
    
    // 组织
    public DbSet<OrganizationUnit> OrganizationUnits => Set<OrganizationUnit>();
    public DbSet<Position> Positions => Set<Position>();
    
    // 员工
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<JobHistory> JobHistories => Set<JobHistory>();
    
    // 身份认证
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<ExternalLogin> ExternalLogins => Set<ExternalLogin>();
    
    // 考勤
    public DbSet<Shift> Shifts => Set<Shift>();
    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
    
    // 假期
    public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
    public DbSet<LeavePolicy> LeavePolicies => Set<LeavePolicy>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<LeaveBalance> LeaveBalances => Set<LeaveBalance>();
    
    // 薪酬
    public DbSet<PayrollPeriod> PayrollPeriods => Set<PayrollPeriod>();
    public DbSet<PayrollItem> PayrollItems => Set<PayrollItem>();
    public DbSet<PayrollRecord> PayrollRecords => Set<PayrollRecord>();
    
    // 报销
    public DbSet<ExpenseRequest> ExpenseRequests => Set<ExpenseRequest>();
    
    // 审计
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // 应用所有配置
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HrevolveDbContext).Assembly);
        
        // 全局查询过滤器 - 多租户隔离
        ConfigureTenantFilter(modelBuilder);
        
        // 全局查询过滤器 - 软删除
        ConfigureSoftDeleteFilter(modelBuilder);
    }
    
    private void ConfigureTenantFilter(ModelBuilder modelBuilder)
    {
        // 为所有AuditableEntity添加租户过滤器
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(HrevolveDbContext)
                    .GetMethod(nameof(SetTenantFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);
                
                method.Invoke(null, [modelBuilder, this]);
            }
        }
    }
    
    private static void SetTenantFilter<T>(ModelBuilder modelBuilder, HrevolveDbContext context) where T : AuditableEntity
    {
        modelBuilder.Entity<T>().HasQueryFilter(e => 
            context._tenantContextAccessor.TenantContext == null || 
            e.TenantId == context._tenantContextAccessor.TenantContext.TenantId);
    }
    
    private void ConfigureSoftDeleteFilter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(HrevolveDbContext)
                    .GetMethod(nameof(SetSoftDeleteFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);
                
                method.Invoke(null, [modelBuilder]);
            }
        }
    }
    
    private static void SetSoftDeleteFilter<T>(ModelBuilder modelBuilder) where T : AuditableEntity
    {
        modelBuilder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var currentUser = _currentUserAccessor.CurrentUser;
        var tenantContext = _tenantContextAccessor.TenantContext;
        var now = DateTime.UtcNow;
        
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = currentUser?.Id;
                    if (tenantContext?.HasTenant == true && entry.Entity.TenantId == Guid.Empty)
                    {
                        entry.Entity.TenantId = tenantContext.TenantId;
                    }
                    break;
                    
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = currentUser?.Id;
                    break;
                    
                case EntityState.Deleted:
                    // 软删除
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = now;
                    entry.Entity.DeletedBy = currentUser?.Id;
                    break;
            }
        }
        
        return base.SaveChangesAsync(cancellationToken);
    }
}

using Hrevolve.Domain.Employees;
using Microsoft.EntityFrameworkCore;

namespace Hrevolve.Infrastructure.Persistence.Repositories;

/// <summary>
/// 员工仓储接口
/// </summary>
public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber, CancellationToken cancellationToken = default);
    Task<Employee?> GetWithJobHistoryAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Employee>> GetByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Employee>> GetDirectReportsAsync(Guid managerId, CancellationToken cancellationToken = default);
    Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken = default);
    Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取员工在指定日期的职位信息（SCD Type 2查询）
    /// </summary>
    Task<JobHistory?> GetJobHistoryAtDateAsync(Guid employeeId, DateOnly date, CancellationToken cancellationToken = default);
}

/// <summary>
/// 员工仓储实现
/// </summary>
public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(HrevolveDbContext context) : base(context)
    {
    }
    
    public async Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(e => e.EmployeeNumber == employeeNumber, cancellationToken);
    }
    
    public async Task<Employee?> GetWithJobHistoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(e => e.JobHistories.Where(j => j.CorrectionStatus != CorrectionStatus.Voided))
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
    
    public async Task<IReadOnlyList<Employee>> GetByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        
        // 通过JobHistory获取当前在该部门的员工
        var employeeIds = await Context.JobHistories
            .Where(j => j.DepartmentId == departmentId 
                        && j.EffectiveStartDate <= today 
                        && j.EffectiveEndDate >= today
                        && j.CorrectionStatus != CorrectionStatus.Voided)
            .Select(j => j.EmployeeId)
            .Distinct()
            .ToListAsync(cancellationToken);
        
        return await DbSet
            .Where(e => employeeIds.Contains(e.Id))
            .ToListAsync(cancellationToken);
    }
    
    public async Task<IReadOnlyList<Employee>> GetDirectReportsAsync(Guid managerId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(e => e.DirectManagerId == managerId)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<JobHistory?> GetJobHistoryAtDateAsync(Guid employeeId, DateOnly date, CancellationToken cancellationToken = default)
    {
        // SCD Type 2 时点查询
        return await Context.JobHistories
            .Where(j => j.EmployeeId == employeeId 
                        && date >= j.EffectiveStartDate 
                        && date <= j.EffectiveEndDate
                        && j.CorrectionStatus != CorrectionStatus.Voided)
            .OrderByDescending(j => j.EffectiveStartDate)
            .FirstOrDefaultAsync(cancellationToken);
    }
}

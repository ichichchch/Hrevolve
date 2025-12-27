using Hrevolve.Domain.Organizations;
using Microsoft.EntityFrameworkCore;

namespace Hrevolve.Infrastructure.Persistence.Repositories;

/// <summary>
/// 组织仓储接口
/// </summary>
public interface IOrganizationRepository
{
    Task<OrganizationUnit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OrganizationUnit?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<OrganizationUnit?> GetWithChildrenAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrganizationUnit>> GetRootUnitsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrganizationUnit>> GetSubTreeAsync(string path, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrganizationUnit>> GetAncestorsAsync(string path, CancellationToken cancellationToken = default);
    Task<OrganizationUnit> AddAsync(OrganizationUnit unit, CancellationToken cancellationToken = default);
    Task UpdateAsync(OrganizationUnit unit, CancellationToken cancellationToken = default);
}

/// <summary>
/// 组织仓储实现
/// </summary>
public class OrganizationRepository : Repository<OrganizationUnit>, IOrganizationRepository
{
    public OrganizationRepository(HrevolveDbContext context) : base(context)
    {
    }
    
    public async Task<OrganizationUnit?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(o => o.Code == code, cancellationToken);
    }
    
    public async Task<OrganizationUnit?> GetWithChildrenAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(o => o.Children)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }
    
    public async Task<IReadOnlyList<OrganizationUnit>> GetRootUnitsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(o => o.ParentId == null)
            .OrderBy(o => o.SortOrder)
            .ToListAsync(cancellationToken);
    }
    
    /// <summary>
    /// 获取子树（利用路径枚举模型）
    /// </summary>
    public async Task<IReadOnlyList<OrganizationUnit>> GetSubTreeAsync(string path, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(o => o.Path.StartsWith(path))
            .OrderBy(o => o.Level)
            .ThenBy(o => o.SortOrder)
            .ToListAsync(cancellationToken);
    }
    
    /// <summary>
    /// 获取祖先节点（利用路径枚举模型）
    /// </summary>
    public async Task<IReadOnlyList<OrganizationUnit>> GetAncestorsAsync(string path, CancellationToken cancellationToken = default)
    {
        // 从路径中提取所有祖先ID
        var ancestorIds = path
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Select(Guid.Parse)
            .ToList();
        
        return await DbSet
            .Where(o => ancestorIds.Contains(o.Id))
            .OrderBy(o => o.Level)
            .ToListAsync(cancellationToken);
    }
}

/// <summary>
/// 职位仓储接口
/// </summary>
public interface IPositionRepository
{
    Task<Position?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Position?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Position>> GetByOrganizationUnitAsync(Guid organizationUnitId, CancellationToken cancellationToken = default);
    Task<Position> AddAsync(Position position, CancellationToken cancellationToken = default);
    Task UpdateAsync(Position position, CancellationToken cancellationToken = default);
}

/// <summary>
/// 职位仓储实现
/// </summary>
public class PositionRepository : Repository<Position>, IPositionRepository
{
    public PositionRepository(HrevolveDbContext context) : base(context)
    {
    }
    
    public async Task<Position?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(p => p.Code == code, cancellationToken);
    }
    
    public async Task<IReadOnlyList<Position>> GetByOrganizationUnitAsync(Guid organizationUnitId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.OrganizationUnitId == organizationUnitId && p.IsActive)
            .ToListAsync(cancellationToken);
    }
}

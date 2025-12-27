using Hrevolve.Domain.Common;

namespace Hrevolve.Domain.Organizations;

/// <summary>
/// 组织单元 - 支持树状组织架构
/// 使用邻接表模型(parent_id) + 路径枚举模型(path)
/// </summary>
public class OrganizationUnit : AuditableEntity
{
    public string Name { get; private set; } = null!;
    public string Code { get; private set; } = null!;
    public string? Description { get; private set; }
    public Guid? ParentId { get; private set; }
    public OrganizationUnit? Parent { get; private set; }
    
    /// <summary>
    /// 路径枚举 - 如 /1/5/23/ 用于快速查询子树
    /// </summary>
    public string Path { get; private set; } = "/";
    
    /// <summary>
    /// 层级深度
    /// </summary>
    public int Level { get; private set; }
    
    /// <summary>
    /// 排序序号
    /// </summary>
    public int SortOrder { get; private set; }
    
    public OrganizationUnitType Type { get; private set; }
    public bool IsActive { get; private set; } = true;
    
    /// <summary>
    /// 负责人ID
    /// </summary>
    public Guid? ManagerId { get; private set; }
    
    private readonly List<OrganizationUnit> _children = [];
    public IReadOnlyCollection<OrganizationUnit> Children => _children.AsReadOnly();
    
    private OrganizationUnit() { }
    
    public static OrganizationUnit Create(
        Guid tenantId,
        string name,
        string code,
        OrganizationUnitType type,
        OrganizationUnit? parent = null)
    {
        var unit = new OrganizationUnit
        {
            TenantId = tenantId,
            Name = name,
            Code = code,
            Type = type,
            ParentId = parent?.Id,
            Parent = parent
        };
        
        unit.UpdatePath(parent);
        return unit;
    }
    
    public void UpdatePath(OrganizationUnit? parent)
    {
        if (parent is null)
        {
            Path = $"/{Id}/";
            Level = 0;
        }
        else
        {
            Path = $"{parent.Path}{Id}/";
            Level = parent.Level + 1;
        }
    }
    
    public void SetManager(Guid managerId) => ManagerId = managerId;
    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
    public void UpdateInfo(string name, string? description)
    {
        Name = name;
        Description = description;
    }
    
    /// <summary>
    /// 判断是否为指定组织的上级
    /// </summary>
    public bool IsAncestorOf(OrganizationUnit other) => other.Path.StartsWith(Path);
    
    /// <summary>
    /// 判断是否为指定组织的下级
    /// </summary>
    public bool IsDescendantOf(OrganizationUnit other) => Path.StartsWith(other.Path);
}

public enum OrganizationUnitType
{
    Company,      // 公司
    Division,     // 事业部
    Department,   // 部门
    Team,         // 团队
    Group         // 小组
}

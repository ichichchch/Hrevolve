namespace Hrevolve.Domain.Employees;

/// <summary>
/// 员工实体 - 员工全生命周期管理
/// </summary>
public class Employee : AuditableEntity
{
    public string EmployeeNumber { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string FullName => $"{LastName}{FirstName}";
    public string? EnglishName { get; private set; }
    public Gender Gender { get; private set; }
    public DateOnly DateOfBirth { get; private set; }
    
    /// <summary>
    /// 身份证号（加密存储）
    /// </summary>
    public string? IdCardNumber { get; private set; }
    
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? PersonalEmail { get; private set; }
    public string? Address { get; private set; }
    
    public EmploymentStatus Status { get; private set; }
    public EmploymentType EmploymentType { get; private set; }
    public DateOnly HireDate { get; private set; }
    public DateOnly? TerminationDate { get; private set; }
    public DateOnly? ProbationEndDate { get; private set; }
    
    /// <summary>
    /// 关联的用户账号ID
    /// </summary>
    public Guid? UserId { get; private set; }
    
    /// <summary>
    /// 直属上级ID
    /// </summary>
    public Guid? DirectManagerId { get; private set; }
    public Employee? DirectManager { get; private set; }
    
    /// <summary>
    /// 当前职位历史记录（通过JobHistory获取当前状态）
    /// </summary>
    private readonly List<JobHistory> _jobHistories = [];
    public IReadOnlyCollection<JobHistory> JobHistories => _jobHistories.AsReadOnly();
    
    private Employee() { }
    
    public static Employee Create(
        Guid tenantId,
        string employeeNumber,
        string firstName,
        string lastName,
        Gender gender,
        DateOnly dateOfBirth,
        DateOnly hireDate,
        EmploymentType employmentType)
    {
        var employee = new Employee
        {
            TenantId = tenantId,
            EmployeeNumber = employeeNumber,
            FirstName = firstName,
            LastName = lastName,
            Gender = gender,
            DateOfBirth = dateOfBirth,
            HireDate = hireDate,
            EmploymentType = employmentType,
            Status = EmploymentStatus.Active
        };
        
        employee.AddDomainEvent(new EmployeeCreatedEvent(employee.Id, tenantId));
        return employee;
    }
    
    public void SetContactInfo(string? email, string? phone, string? personalEmail, string? address)
    {
        Email = email;
        Phone = phone;
        PersonalEmail = personalEmail;
        Address = address;
    }
    
    public void SetIdCardNumber(string encryptedIdCard) => IdCardNumber = encryptedIdCard;
    
    public void SetDirectManager(Guid? managerId) => DirectManagerId = managerId;
    
    public void LinkUser(Guid userId) => UserId = userId;
    
    public void Terminate(DateOnly terminationDate)
    {
        Status = EmploymentStatus.Terminated;
        TerminationDate = terminationDate;
        AddDomainEvent(new EmployeeTerminatedEvent(Id, TenantId, terminationDate));
    }
    
    public void PassProbation()
    {
        if (Status == EmploymentStatus.Active && ProbationEndDate.HasValue)
        {
            ProbationEndDate = null;
            AddDomainEvent(new EmployeeProbationPassedEvent(Id, TenantId));
        }
    }
}

public enum Gender
{
    Male,
    Female,
    Other
}

public enum EmploymentStatus
{
    Active,       // 在职
    OnLeave,      // 休假中
    Suspended,    // 停职
    Terminated    // 离职
}

public enum EmploymentType
{
    FullTime,     // 全职
    PartTime,     // 兼职
    Contract,     // 合同工
    Intern,       // 实习生
    Consultant    // 顾问
}

// 领域事件
public record EmployeeCreatedEvent(Guid EmployeeId, Guid TenantId) : DomainEvent;
public record EmployeeTerminatedEvent(Guid EmployeeId, Guid TenantId, DateOnly TerminationDate) : DomainEvent;
public record EmployeeProbationPassedEvent(Guid EmployeeId, Guid TenantId) : DomainEvent;

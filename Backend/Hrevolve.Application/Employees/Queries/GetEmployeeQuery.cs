using Hrevolve.Domain.Employees;
using Hrevolve.Infrastructure.Persistence.Repositories;
using Hrevolve.Shared.Exceptions;
using Hrevolve.Shared.Results;
using MediatR;

namespace Hrevolve.Application.Employees.Queries;

/// <summary>
/// 员工详情DTO
/// </summary>
public record EmployeeDto
{
    public Guid Id { get; init; }
    public string EmployeeNumber { get; init; } = null!;
    public string FullName { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? EnglishName { get; init; }
    public string Gender { get; init; } = null!;
    public DateOnly DateOfBirth { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string Status { get; init; } = null!;
    public string EmploymentType { get; init; } = null!;
    public DateOnly HireDate { get; init; }
    public DateOnly? TerminationDate { get; init; }
    public Guid? DirectManagerId { get; init; }
    public CurrentJobDto? CurrentJob { get; init; }
}

/// <summary>
/// 当前职位信息DTO
/// </summary>
public record CurrentJobDto
{
    public Guid PositionId { get; init; }
    public Guid DepartmentId { get; init; }
    public decimal BaseSalary { get; init; }
    public string? Grade { get; init; }
    public DateOnly EffectiveDate { get; init; }
}

/// <summary>
/// 获取员工详情查询
/// </summary>
public record GetEmployeeQuery(Guid EmployeeId) : IRequest<Result<EmployeeDto>>;

/// <summary>
/// 获取员工详情查询处理器
/// </summary>
public class GetEmployeeQueryHandler : IRequestHandler<GetEmployeeQuery, Result<EmployeeDto>>
{
    private readonly IEmployeeRepository _employeeRepository;
    
    public GetEmployeeQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }
    
    public async Task<Result<EmployeeDto>> Handle(GetEmployeeQuery request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        
        if (employee == null)
        {
            throw new EntityNotFoundException(nameof(Employee), request.EmployeeId);
        }
        
        // 获取当前职位信息
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var currentJob = await _employeeRepository.GetJobHistoryAtDateAsync(request.EmployeeId, today, cancellationToken);
        
        var dto = new EmployeeDto
        {
            Id = employee.Id,
            EmployeeNumber = employee.EmployeeNumber,
            FullName = employee.FullName,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            EnglishName = employee.EnglishName,
            Gender = employee.Gender.ToString(),
            DateOfBirth = employee.DateOfBirth,
            Email = employee.Email,
            Phone = employee.Phone,
            Status = employee.Status.ToString(),
            EmploymentType = employee.EmploymentType.ToString(),
            HireDate = employee.HireDate,
            TerminationDate = employee.TerminationDate,
            DirectManagerId = employee.DirectManagerId,
            CurrentJob = currentJob != null ? new CurrentJobDto
            {
                PositionId = currentJob.PositionId,
                DepartmentId = currentJob.DepartmentId,
                BaseSalary = currentJob.BaseSalary,
                Grade = currentJob.Grade,
                EffectiveDate = currentJob.EffectiveStartDate
            } : null
        };
        
        return Result.Success(dto);
    }
}

/// <summary>
/// 获取员工历史时点状态查询
/// </summary>
public record GetEmployeeAtDateQuery(Guid EmployeeId, DateOnly Date) : IRequest<Result<EmployeeDto>>;

/// <summary>
/// 获取员工历史时点状态查询处理器
/// </summary>
public class GetEmployeeAtDateQueryHandler : IRequestHandler<GetEmployeeAtDateQuery, Result<EmployeeDto>>
{
    private readonly IEmployeeRepository _employeeRepository;
    
    public GetEmployeeAtDateQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }
    
    public async Task<Result<EmployeeDto>> Handle(GetEmployeeAtDateQuery request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        
        if (employee == null)
        {
            throw new EntityNotFoundException(nameof(Employee), request.EmployeeId);
        }
        
        // 获取指定日期的职位信息（SCD Type 2查询）
        var jobAtDate = await _employeeRepository.GetJobHistoryAtDateAsync(request.EmployeeId, request.Date, cancellationToken);
        
        var dto = new EmployeeDto
        {
            Id = employee.Id,
            EmployeeNumber = employee.EmployeeNumber,
            FullName = employee.FullName,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            EnglishName = employee.EnglishName,
            Gender = employee.Gender.ToString(),
            DateOfBirth = employee.DateOfBirth,
            Email = employee.Email,
            Phone = employee.Phone,
            Status = employee.Status.ToString(),
            EmploymentType = employee.EmploymentType.ToString(),
            HireDate = employee.HireDate,
            TerminationDate = employee.TerminationDate,
            DirectManagerId = employee.DirectManagerId,
            CurrentJob = jobAtDate != null ? new CurrentJobDto
            {
                PositionId = jobAtDate.PositionId,
                DepartmentId = jobAtDate.DepartmentId,
                BaseSalary = jobAtDate.BaseSalary,
                Grade = jobAtDate.Grade,
                EffectiveDate = jobAtDate.EffectiveStartDate
            } : null
        };
        
        return Result.Success(dto);
    }
}

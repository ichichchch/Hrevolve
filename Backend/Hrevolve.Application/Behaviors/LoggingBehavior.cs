using System.Diagnostics;
using Hrevolve.Shared.Identity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hrevolve.Application.Behaviors;

/// <summary>
/// 日志管道行为 - 记录请求处理日志
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserAccessor _currentUserAccessor;
    
    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        ICurrentUserAccessor currentUserAccessor)
    {
        _logger = logger;
        _currentUserAccessor = currentUserAccessor;
    }
    
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUserAccessor.CurrentUser?.Id;
        var tenantId = _currentUserAccessor.CurrentUser?.TenantId;
        
        _logger.LogInformation(
            "处理请求 {RequestName} - 用户: {UserId}, 租户: {TenantId}",
            requestName, userId, tenantId);
        
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var response = await next();
            
            stopwatch.Stop();
            
            _logger.LogInformation(
                "请求 {RequestName} 处理完成 - 耗时: {ElapsedMilliseconds}ms",
                requestName, stopwatch.ElapsedMilliseconds);
            
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            _logger.LogError(ex,
                "请求 {RequestName} 处理失败 - 耗时: {ElapsedMilliseconds}ms, 错误: {ErrorMessage}",
                requestName, stopwatch.ElapsedMilliseconds, ex.Message);
            
            throw;
        }
    }
}

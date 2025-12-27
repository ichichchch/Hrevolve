namespace Hrevolve.Shared.Results;

/// <summary>
/// 操作结果 - 统一返回格式
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }
    public string? ErrorCode { get; }
    
    protected Result(bool isSuccess, string? error, string? errorCode)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorCode = errorCode;
    }
    
    public static Result Success() => new(true, null, null);
    public static Result Failure(string error, string? errorCode = null) => new(false, error, errorCode);
    
    public static Result<T> Success<T>(T value) => new(value, true, null, null);
    public static Result<T> Failure<T>(string error, string? errorCode = null) => new(default, false, error, errorCode);
}

/// <summary>
/// 带数据的操作结果
/// </summary>
public class Result<T> : Result
{
    public T? Value { get; }
    
    internal Result(T? value, bool isSuccess, string? error, string? errorCode)
        : base(isSuccess, error, errorCode)
    {
        Value = value;
    }
    
    public static implicit operator Result<T>(T value) => Success(value);
}

/// <summary>
/// 分页结果
/// </summary>
public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int TotalCount { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
    
    public PagedResult(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}

/// <summary>
/// 分页请求
/// </summary>
public class PagedRequest
{
    private int _pageNumber = 1;
    private int _pageSize = 20;
    
    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }
    
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1 ? 20 : (value > 100 ? 100 : value);
    }
    
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}

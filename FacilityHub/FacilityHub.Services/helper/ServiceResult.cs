using System.Net;

namespace FacilityHub.Services;

public class ServiceResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public string[] Errors { get; set; } = Array.Empty<string>();
    public HttpStatusCode StatusCode { get; set; }


    public static ServiceResult Success(string message, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new ServiceResult
        {
            IsSuccess = true,
            Message = message,
            StatusCode = statusCode
        };
    }

    public static ServiceResult Failed(string message, string errorCode, string[] errors, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new ServiceResult
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = errorCode,
            Errors = errors,
            StatusCode = statusCode
        };
    }
}

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; set; }


    public static ServiceResult<T> Success(T data, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new ServiceResult<T>
        {
            IsSuccess = true,
            Data = data,
            StatusCode = statusCode
        };
    }

    public static ServiceResult<T> Success(T data, string message, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new ServiceResult<T>
        {
            IsSuccess = true,
            Data = data,
            Message = message,
        };
    }
    // 
    public static ServiceResult<T> Failed(string message, string ErrorCode, string[] Errors,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new ServiceResult<T>
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = ErrorCode,
            Errors = Errors,
            StatusCode = statusCode
        };
    }
}
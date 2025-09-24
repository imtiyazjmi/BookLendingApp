namespace BookLendingApp.Models;

public class ApiResponse<T>
{
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public string? Error { get; set; }

    public static ApiResponse<T> Success(T data, string message = "Success", int code = 200)
    {
        return new ApiResponse<T>
        {
            Code = code,
            Message = message,
            Data = data,
            Error = null
        };
    }

    public static ApiResponse<T> Failure(string error, string message = "Error", int code = 400)
    {
        return new ApiResponse<T>
        {
            Code = code,
            Message = message,
            Data = default,
            Error = error
        };
    }
}
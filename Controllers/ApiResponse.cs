using System;

namespace e_commerce_web_api.Controllers;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string>? Errors { get; set; }
    public int StatusCode { get; set; }
    public DateTime TimeStamp { get; set; }
    public T? Result { get; set; }


    //constructor for result 
    private ApiResponse(bool success, string message, List<string>? errors, int statusCode, T? data)
    {
        Success = success;
        Message = message;
        Errors = errors;
        StatusCode = statusCode;
        TimeStamp = DateTime.UtcNow;
        Result = data;

    }

    // static method for creating a successfull result
    public static ApiResponse<T> SuccessResponse(int statusCode, string message, T data)
    {
        return new ApiResponse<T>(true, message, null, statusCode, data);
    }

    // static method for create errors result
    public static ApiResponse<T> ErrorResponse(string message, int statusCode, List<string> errors)
    {
        return new ApiResponse<T>(false, message, errors, statusCode, default(T));
    }

}

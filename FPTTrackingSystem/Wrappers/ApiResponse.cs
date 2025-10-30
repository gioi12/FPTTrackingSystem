namespace FPTTrackingSystem.Wrappers
{
    public class ApiResponse<T>
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }

        public ApiResponse() { }

        public ApiResponse(int status, string message, T? data = default)
        {
            Status = status;
            Message = message;
            Data = data;
        }
        public static ApiResponse<T> Success(T? data, string message = "Success", int code = 200)
             => new ApiResponse<T>(code, message, data);

        public static ApiResponse<T> Fail(string message, int code = 400)
            => new ApiResponse<T>(code, message);

        public static ApiResponse<T> Unauthorized(string message = "Unauthorized")
            => new ApiResponse<T>(401, message);

        public static ApiResponse<T> Forbidden(string message = "Forbidden")
            => new ApiResponse<T>(403, message);

        public static ApiResponse<T> InternalError(string message = "Internal Server Error")
            => new ApiResponse<T>(500, message);
    }
}
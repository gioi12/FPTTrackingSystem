namespace FPTTrackingSystem.Wrappers
{
    public class ApiResponse<T>
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }

        public ApiResponse(int code, string message, T? data = default)
        {
            Code = code;
            Message = message;
            Data = data;
        }
        public static ApiResponse<T> Success(T? data, string message = "Success", int code = 200)
             => new ApiResponse<T>(code, message, data);

        public static ApiResponse<T> Fail(string message, int code = 400)
            => new ApiResponse<T>(code, message);

        public static ApiResponse<T> Unauthorized(string message = "Unauthorized")
            => new ApiResponse<T>(401, message);

        public static ApiResponse<T> InternalError(string message = "Internal Server Error")
            => new ApiResponse<T>(500, message);
    }
}

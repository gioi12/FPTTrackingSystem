using Azure;
using FPTTrackingSystem.Wrappers;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace FPTTrackingSystem.Middlewares
{
    public class GlobalErrorMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalErrorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var statusCode = HttpStatusCode.InternalServerError;
                object? responseBody = null;

                switch (ex)
                {
                    case ValidationException ve:
                        statusCode = HttpStatusCode.BadRequest; // 400
                        responseBody = ApiResponse<object>.Fail(ve.Message, (int)statusCode);
                        break;

                    case UnauthorizedAccessException ue:
                        statusCode = HttpStatusCode.Unauthorized; // 401
                        responseBody = ApiResponse<object>.Fail(ue.Message, (int)statusCode);
                        break;

                    case KeyNotFoundException knf:
                        statusCode = HttpStatusCode.NotFound; // 404
                        responseBody = ApiResponse<object>.Fail(knf.Message, (int)statusCode);
                        break;

                    default:
                        responseBody = ApiResponse<object>.InternalError(ex.Message);
                        break;
                }

                context.Response.StatusCode = (int)statusCode;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(responseBody);
            }
        }
    }
}
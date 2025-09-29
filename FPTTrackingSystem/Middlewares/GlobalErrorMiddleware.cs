using System.Net;
using FPTTrackingSystem.Wrappers;

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
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var response = ApiResponse<object>.InternalError(ex.Message);
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }

    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalErrorHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalErrorMiddleware>();
        }
    }
}
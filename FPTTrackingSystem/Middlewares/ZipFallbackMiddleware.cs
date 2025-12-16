using System.IO.Compression;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

public class ZipFallbackMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _uploadsRoot;

    public ZipFallbackMiddleware(RequestDelegate next, string uploadsRoot)
    {
        _next = next;
        _uploadsRoot = uploadsRoot;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;

        // Chỉ xử lý /uploads
        if (!path.StartsWith("/uploads", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }


        var served = await TryServeFromZip(context, path);
        if (served) 
        {
            return;
        }

        // Nếu không có trong ZIP, gọi StaticFiles xử lý
        await _next(context);
    }

    private async Task<bool> TryServeFromZip(HttpContext context, string path) 
    {
        var segments = path.TrimStart('/').Split('/');

        if (segments.Length < 3)
        {
            return false;  
        }

        string semesterFolder = segments[1];
        string zipPath = Path.Combine(_uploadsRoot, semesterFolder + ".zip");

        if (!File.Exists(zipPath))
        {
            return false;  
        }

        string insideZipPath = string.Join('/', segments.Skip(1));

        try
        {
            using var zip = ZipFile.OpenRead(zipPath);

            var entry = zip.GetEntry(insideZipPath.Replace('\\', '/'))
                     ?? zip.GetEntry(insideZipPath.Replace('\\', '/').TrimStart('/'));

            if (entry == null)
            {
                return false; 
            }

            if (context.Response.HasStarted)
            {
                return false;
            }

            context.Response.StatusCode = 200;
            context.Response.ContentType = GetContentType(insideZipPath);

            context.Response.ContentLength = entry.Length;

            context.Response.Headers.Append("Accept-Ranges", "bytes");

            context.Response.Headers.Append("Access-Control-Allow-Origin", "*");

            context.Response.Headers.Append("Cache-Control", "public, max-age=31536000");
            context.Response.Headers.Append("ETag", $"\"{entry.Crc32:X8}\"");

            context.Response.Headers.Append("Content-Disposition",
                $"inline; filename=\"{Path.GetFileName(insideZipPath)}\"");

            using var stream = entry.Open();
            await stream.CopyToAsync(context.Response.Body, context.RequestAborted); 

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error serving from ZIP: {ex.Message}");
            return false; 
        }
    }

    private static string GetContentType(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ext switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".txt" => "text/plain",
            ".html" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".json" => "application/json",
            ".zip" => "application/zip",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            _ => "application/octet-stream"
        };
    }
}

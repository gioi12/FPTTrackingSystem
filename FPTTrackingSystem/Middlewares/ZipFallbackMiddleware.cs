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

        if (!path.StartsWith("/uploads", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        // Kiểm tra file vật lý trước
        var physicalPath = GetPhysicalFilePath(path);
        if (File.Exists(physicalPath))
        {
            // File thật tồn tại → để StaticFiles xử lý
            await _next(context);
            return;
        }

        // File thật không tồn tại → thử ZIP
        var served = await TryServeFromZip(context, path);
        if (served)
        {
            return;
        }

        // Không tìm thấy ở cả 2 nơi → 404
        context.Response.StatusCode = 404;
    }

    private string GetPhysicalFilePath(string requestPath)
    {
        try
        {
            var relativePath = requestPath.Substring("/uploads".Length).TrimStart('/');
            var fullPath = Path.Combine(_uploadsRoot, relativePath);

            var normalizedPath = Path.GetFullPath(fullPath);
            var normalizedRoot = Path.GetFullPath(_uploadsRoot);

            if (!normalizedPath.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return normalizedPath;
        }
        catch
        {
            return null;
        }
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
            context.Response.Headers["Accept-Ranges"] = "bytes";
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            context.Response.Headers["Cache-Control"] = "public, max-age=31536000";
            context.Response.Headers["ETag"] = $"\"{entry.Crc32:X8}\"";
            context.Response.Headers["Content-Disposition"] = $"inline; filename=\"{Path.GetFileName(insideZipPath)}\"";

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
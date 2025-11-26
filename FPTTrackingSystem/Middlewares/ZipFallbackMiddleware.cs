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

        // Gọi StaticFiles trước
        await _next(context);

        // Nếu đã serve thành công, return
        if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
        {
            return;
        }

        // Nếu 404 và chưa ghi response → thử ZIP
        if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
        {
            await TryServeFromZip(context, path);
        }
    }

    private async Task TryServeFromZip(HttpContext context, string path)
    {
        // path = "/uploads/Fall25/Group1/Fall25_GroupGroup1.pdf"
        var segments = path.TrimStart('/').Split('/');

        // segments = ["uploads", "Fall25", "Group1", "Fall25_GroupGroup1.pdf"]
        if (segments.Length < 3)
        {
            return;
        }

        string semesterFolder = segments[1]; // "Fall25"
        string zipPath = Path.Combine(_uploadsRoot, semesterFolder + ".zip");

        if (!File.Exists(zipPath))
        {
            return;
        }

        // Path trong ZIP: GIỮ NGUYÊN từ Fall25 trở đi
        // Vì bên trong ZIP có cấu trúc: Fall25/Group1/...
        string insideZipPath = string.Join('/', segments.Skip(1)); // Bỏ "uploads"

        try
        {
            using var zip = ZipFile.OpenRead(zipPath);

            // Thử cả 2 cách: với và không có thư mục gốc
            var entry = zip.GetEntry(insideZipPath.Replace('\\', '/'))
                     ?? zip.GetEntry(insideZipPath.Replace('\\', '/').TrimStart('/'));

            if (entry == null)
            {
                return;
            }

            context.Response.StatusCode = 200;
            context.Response.ContentType = GetContentType(insideZipPath);
            context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
            context.Response.Headers.Append("Content-Disposition",
                $"inline; filename=\"{Path.GetFileName(insideZipPath)}\"");

            using var stream = entry.Open();
            await stream.CopyToAsync(context.Response.Body);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error serving from ZIP: {ex.Message}");
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

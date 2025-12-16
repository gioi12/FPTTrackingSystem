using System.IO.Compression;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

public class ZipFallbackMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _uploadsRoot;
    private readonly IContentTypeProvider _contentTypeProvider;

    public ZipFallbackMiddleware(RequestDelegate next, string uploadsRoot)
    {
        _next = next;
        _uploadsRoot = uploadsRoot;
        _contentTypeProvider = new FileExtensionContentTypeProvider();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;

        // Không phải /uploads → skip
        if (string.IsNullOrEmpty(path) || !path.StartsWith("/uploads", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        // Bước 1: Thử serve file vật lý trước
        var physicalPath = GetPhysicalFilePath(path);
        if (!string.IsNullOrEmpty(physicalPath) && File.Exists(physicalPath))
        {
            await ServePhysicalFile(context, physicalPath);
            return;
        }

        // Bước 2: Thử serve từ ZIP
        var served = await TryServeFromZip(context, path);
        if (served)
        {
            return;
        }

        // Bước 3: Không tìm thấy → 404
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("File not found");
    }

    private string GetPhysicalFilePath(string requestPath)
    {
        try
        {
            // requestPath = "/uploads/Fall25/Group1/file.pdf"
            // Bỏ "/uploads" prefix
            var relativePath = requestPath.Substring("/uploads".Length).TrimStart('/');

            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return null;
            }

            // Tạo full path
            var fullPath = Path.Combine(_uploadsRoot, relativePath);

            // Security check: không cho thoát ra ngoài uploads folder
            var normalizedPath = Path.GetFullPath(fullPath);
            var normalizedRoot = Path.GetFullPath(_uploadsRoot);

            if (!normalizedPath.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return normalizedPath;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting physical path: {ex.Message}");
            return null;
        }
    }

    private async Task ServePhysicalFile(HttpContext context, string physicalPath)
    {
        try
        {
            var fileInfo = new FileInfo(physicalPath);

            if (!fileInfo.Exists)
            {
                return;
            }

            // Get content type
            if (!_contentTypeProvider.TryGetContentType(physicalPath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            // Set response headers
            context.Response.Clear();
            context.Response.StatusCode = 200;
            context.Response.ContentType = contentType;
            context.Response.ContentLength = fileInfo.Length;

            var fileName = Path.GetFileName(physicalPath);
            var shouldInline = ShouldDisplayInline(contentType);

            context.Response.Headers["Content-Disposition"] = shouldInline
                ? $"inline; filename=\"{fileName}\""
                : $"attachment; filename=\"{fileName}\"";

            context.Response.Headers["Accept-Ranges"] = "bytes";
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            context.Response.Headers["Cache-Control"] = "public, max-age=31536000";

            // Stream file với buffer 64KB
            using var fileStream = new FileStream(
                physicalPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 65536,
                useAsync: true
            );

            await fileStream.CopyToAsync(context.Response.Body, context.RequestAborted);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error serving physical file {physicalPath}: {ex.Message}");
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = 500;
            }
        }
    }

    private async Task<bool> TryServeFromZip(HttpContext context, string path)
    {
        try
        {
            // path = "/uploads/Fall25/Group1/file.pdf"
            var segments = path.TrimStart('/').Split('/');

            // segments = ["uploads", "Fall25", "Group1", "file.pdf"]
            if (segments.Length < 3)
            {
                return false;
            }

            string semesterFolder = segments[1]; // "Fall25"
            string zipPath = Path.Combine(_uploadsRoot, semesterFolder + ".zip");

            if (!File.Exists(zipPath))
            {
                return false;
            }

            // Path trong ZIP: bỏ "uploads" prefix
            // insideZipPath = "Fall25/Group1/file.pdf"
            string insideZipPath = string.Join('/', segments.Skip(1));

            using var zip = ZipFile.OpenRead(zipPath);

            // Thử nhiều cách tìm entry
            var entry = zip.GetEntry(insideZipPath.Replace('\\', '/'))
                     ?? zip.GetEntry(insideZipPath.Replace('\\', '/').TrimStart('/'))
                     ?? zip.Entries.FirstOrDefault(e =>
                         e.FullName.Replace('\\', '/').Equals(insideZipPath.Replace('\\', '/'),
                         StringComparison.OrdinalIgnoreCase));

            if (entry == null)
            {
                Console.WriteLine($"Entry not found in ZIP: {insideZipPath}");
                return false;
            }

            // Get content type
            if (!_contentTypeProvider.TryGetContentType(entry.Name, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            // Set response headers
            context.Response.Clear();
            context.Response.StatusCode = 200;
            context.Response.ContentType = contentType;
            context.Response.ContentLength = entry.Length;

            var fileName = Path.GetFileName(entry.Name);
            var shouldInline = ShouldDisplayInline(contentType);

            context.Response.Headers["Content-Disposition"] = shouldInline
                ? $"inline; filename=\"{fileName}\""
                : $"attachment; filename=\"{fileName}\"";

            context.Response.Headers["Accept-Ranges"] = "bytes";
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            context.Response.Headers["Cache-Control"] = "public, max-age=31536000";
            context.Response.Headers["ETag"] = $"\"{entry.Crc32:X8}\"";

            // Copy từ ZIP entry sang response body
            using var zipStream = entry.Open();
            await zipStream.CopyToAsync(context.Response.Body, 65536, context.RequestAborted);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error serving from ZIP for path {path}: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return false;
        }
    }

    private static bool ShouldDisplayInline(string contentType)
    {
        return contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) ||
               contentType.StartsWith("text/", StringComparison.OrdinalIgnoreCase) ||
               contentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase) ||
               contentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase) ||
               contentType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase);
    }
}

using DataTranferObjects.Admin;
using System.IO.Compression;

namespace FPTTrackingSystem.Services.Admin
{
    public class StorageService : IStorageService
    {
        /// <summary>
        /// Lấy danh sách tất cả các kỳ học với thông tin dung lượng
        /// </summary>
        public List<SemesterStorageInfo> GetAllSemesters(string uploadsRoot)
        {
            if (!Directory.Exists(uploadsRoot))
                throw new DirectoryNotFoundException($"Uploads folder không tồn tại: {uploadsRoot}");

            var semesters = new List<SemesterStorageInfo>();
            var processedNames = new HashSet<string>();

            // Lấy tất cả folders
            foreach (var dir in Directory.GetDirectories(uploadsRoot))
            {
                var name = Path.GetFileName(dir);
                if (!processedNames.Contains(name))
                {
                    processedNames.Add(name);
                    semesters.Add(GetSemesterInfo(uploadsRoot, name));
                }
            }

            // Lấy tất cả zip files không có folder tương ứng
            foreach (var file in Directory.GetFiles(uploadsRoot, "*.zip"))
            {
                var nameWithoutExt = Path.GetFileNameWithoutExtension(file);
                if (!processedNames.Contains(nameWithoutExt))
                {
                    processedNames.Add(nameWithoutExt);
                    semesters.Add(GetSemesterInfo(uploadsRoot, nameWithoutExt));
                }
            }

            return semesters.OrderBy(s => s.Name).ToList();
        }

        private SemesterStorageInfo GetSemesterInfo(string uploadsRoot, string semesterName)
        {
            var info = new SemesterStorageInfo { Name = semesterName };

            var folderPath = Path.Combine(uploadsRoot, semesterName);
            var zipPath = Path.Combine(uploadsRoot, semesterName + ".zip");

            info.HasFolder = Directory.Exists(folderPath);
            info.HasZipFile = File.Exists(zipPath);

            if (info.HasFolder)
                info.FolderSize = GetDirectorySize(folderPath);

            if (info.HasZipFile)
                info.ZipSize = new FileInfo(zipPath).Length;

            return info;
        }

       
        private FileNode BuildFileNode(string fullPath, string relativePath)
        {
            var info = new DirectoryInfo(fullPath);
            var node = new FileNode
            {
                Name = Path.GetFileName(fullPath),
                Path = relativePath,
                IsDirectory = true,
                LastModified = info.LastWriteTime,
                Size = 0
            };

            try
            {
                // Thêm sub-directories
                foreach (var dir in Directory.GetDirectories(fullPath))
                {
                    var subRelativePath = Path.Combine(relativePath, Path.GetFileName(dir));
                    node.Children.Add(BuildFileNode(dir, subRelativePath));
                }

                // Thêm files
                foreach (var file in Directory.GetFiles(fullPath))
                {
                    var fileInfo = new FileInfo(file);
                    node.Children.Add(new FileNode
                    {
                        Name = fileInfo.Name,
                        Path = Path.Combine(relativePath, fileInfo.Name),
                        IsDirectory = false,
                        Size = fileInfo.Length,
                        SizeFormatted = FormatSize(fileInfo.Length),
                        LastModified = fileInfo.LastWriteTime
                    });
                }

                // Tính tổng size
                node.Size = node.Children.Sum(c => c.Size);
                node.SizeFormatted = FormatSize(node.Size);
            }
            catch (UnauthorizedAccessException)
            {
                // Skip folders không có quyền truy cập
            }

            return node;
        }

        /// <summary>
        /// Zip một folder
        /// </summary>
        public async Task<OperationResult> ZipFolderAsync(string uploadsRoot, string folderName)
        {
            try
            {
                var folderPath = Path.Combine(uploadsRoot, folderName);
                var zipPath = Path.Combine(uploadsRoot, folderName + ".zip");

                if (!Directory.Exists(folderPath))
                    return new OperationResult { Success = false, Message = $"Folder không tồn tại: {folderName}" };

                if (File.Exists(zipPath))
                    return new OperationResult { Success = false, Message = $"File ZIP đã tồn tại: {folderName}.zip" };

                await Task.Run(() => ZipFile.CreateFromDirectory(folderPath, zipPath, CompressionLevel.Optimal, true));

                var zipSize = new FileInfo(zipPath).Length;

                // Xóa folder sau khi zip thành công
                Directory.Delete(folderPath, true);

                return new OperationResult
                {
                    Success = true,
                    Message = $"Đã zip thành công: {folderName}.zip ({FormatSize(zipSize)}) - Đã xóa folder gốc",
                    Data = new { ZipPath = zipPath, Size = zipSize }
                };
            }
            catch (Exception ex)
            {
                return new OperationResult { Success = false, Message = $"Lỗi khi zip: {ex.Message}" };
            }
        }

        /// <summary>
        /// Unzip một file archive
        /// </summary>
        public async Task<OperationResult> UnzipArchiveAsync(string uploadsRoot, string archiveFileName, bool deleteAfter = false)
        {
            try
            {
                // Tự động thêm .zip nếu user không nhập
                if (!archiveFileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    archiveFileName += ".zip";
                }

                var archivePath = Path.Combine(uploadsRoot, archiveFileName);

                if (!File.Exists(archivePath))
                    return new OperationResult { Success = false, Message = $"File không tồn tại: {archiveFileName}" };

                if (!archivePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                    return new OperationResult { Success = false, Message = "Chỉ hỗ trợ file .zip" };

                var folderName = Path.GetFileNameWithoutExtension(archiveFileName);
                var extractPath = Path.Combine(uploadsRoot, folderName);

                if (Directory.Exists(extractPath))
                    return new OperationResult { Success = false, Message = $"Folder đã tồn tại: {folderName}" };

                await Task.Run(() => ZipFile.ExtractToDirectory(archivePath, extractPath));

                // Xóa file zip sau khi giải nén thành công
                if (deleteAfter)
                {
                    File.Delete(archivePath);
                }

                var folderSize = GetDirectorySize(extractPath);

                return new OperationResult
                {
                    Success = true,
                    Message = $"Đã giải nén thành công: {folderName} ({FormatSize(folderSize)}){(deleteAfter ? " - Đã xóa file zip" : "")}",
                    Data = new { ExtractPath = extractPath, Size = folderSize, ArchiveDeleted = deleteAfter }
                };
            }
            catch (Exception ex)
            {
                return new OperationResult { Success = false, Message = $"Lỗi khi giải nén: {ex.Message}" };
            }
        }

       

        /// <summary>
        /// Tính dung lượng thư mục
        /// </summary>
        public long GetDirectorySize(string path)
        {
            if (!Directory.Exists(path))
                return 0;

            var dirInfo = new DirectoryInfo(path);
            long size = 0;

            try
            {
                // Files trong thư mục hiện tại
                size = dirInfo.GetFiles().Sum(file => file.Length);

                // Recursive: tất cả sub-directories
                foreach (var dir in dirInfo.GetDirectories())
                {
                    size += GetDirectorySize(dir.FullName);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Skip folders không có quyền truy cập
            }

            return size;
        }

        /// <summary>
        /// Lấy danh sách các nhóm trong một kỳ học (có phân trang)
        /// </summary>
        public PagedResult<GroupInfo> GetGroupsBySemester(string uploadsRoot, string semesterName, int pageNumber = 1, int pageSize = 10)
        {
            var semesterPath = Path.Combine(uploadsRoot, semesterName);

            if (!Directory.Exists(semesterPath))
                throw new DirectoryNotFoundException($"Folder kỳ học không tồn tại: {semesterName}");

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // Giới hạn tối đa 100 items/page

            var allGroups = new List<GroupInfo>();

            foreach (var groupDir in Directory.GetDirectories(semesterPath))
            {
                var groupName = Path.GetFileName(groupDir);
                var dirInfo = new DirectoryInfo(groupDir);

                // Tìm file PDF đầu tiên trong folder nhóm (không recursive)
                var pdfFile = Directory.GetFiles(groupDir, "*.pdf", SearchOption.TopDirectoryOnly).FirstOrDefault();

                var groupInfo = new GroupInfo
                {
                    GroupName = groupName,
                    Path = $"{semesterName}/{groupName}",
                    Size = GetDirectorySize(groupDir),
                    FileCount = Directory.GetFiles(groupDir, "*.*", SearchOption.AllDirectories).Length,
                    SubFolderCount = Directory.GetDirectories(groupDir, "*", SearchOption.AllDirectories).Length,
                    LastModified = dirInfo.LastWriteTime
                };

                groupInfo.SizeFormatted = FormatSize(groupInfo.Size);

                if (pdfFile != null)
                {
                    groupInfo.PdfFileName = Path.GetFileName(pdfFile);
                    groupInfo.PdfFilePath = $"/uploads/{semesterName}/{groupName}/{groupInfo.PdfFileName}";
                }

                allGroups.Add(groupInfo);
            }

            // Sắp xếp theo tên nhóm
            var orderedGroups = allGroups.OrderBy(g => g.GroupName).ToList();

            // Phân trang
            var totalCount = orderedGroups.Count;
            var items = orderedGroups
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<GroupInfo>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }


        private static string FormatSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}

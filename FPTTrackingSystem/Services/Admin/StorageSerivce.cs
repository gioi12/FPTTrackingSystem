using DataTranferObjects.Admin;
using System.IO.Compression;

namespace FPTTrackingSystem.Services.Admin
{
    public class StorageService : IStorageService
    {
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
            {
                info.FolderSize = GetDirectorySize(folderPath);

                // Sử dụng HashSet để tránh đếm trùng
                var allItems = new HashSet<string>();

                // Thêm tất cả subfolder hiện có
                foreach (var subDir in Directory.GetDirectories(folderPath, "*", SearchOption.TopDirectoryOnly))
                {
                    var subDirName = Path.GetFileName(subDir);
                    allItems.Add(subDirName);
                }

                // Thêm tất cả file zip (đại diện cho folder đã bị zip và có thể đã xóa)
                foreach (var zipFile in Directory.GetFiles(folderPath, "*.zip", SearchOption.TopDirectoryOnly))
                {
                    var zipNameWithoutExt = Path.GetFileNameWithoutExtension(zipFile);
                    allItems.Add(zipNameWithoutExt);
                }

                // Tổng số = folder + zip (không trùng lặp)
                info.TotalSubFolders = allItems.Count;

                // Đếm số item đã bị zip
                info.ZippedSubFolders = 0;
                foreach (var itemName in allItems)
                {
                    var zipFilePath = Path.Combine(folderPath, itemName + ".zip");
                    if (File.Exists(zipFilePath))
                    {
                        info.ZippedSubFolders++;
                    }
                }

                // Format chuỗi hiển thị: "số đã zip / tổng số"
                info.ZipFolder = $"{info.ZippedSubFolders}/{info.TotalSubFolders}";
            }
            else
            {
                info.ZipFolder = "N/A";
                info.TotalSubFolders = 0;
                info.ZippedSubFolders = 0;
            }

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
        private static void AddDirectoryToZip(
    ZipArchive zip,
    string sourceDir,
    string entryRoot)
        {
            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var entryName = Path.Combine(entryRoot, Path.GetFileName(file));
                zip.CreateEntryFromFile(file, entryName);
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                var dirName = Path.GetFileName(dir);
                AddDirectoryToZip(zip, dir, Path.Combine(entryRoot, dirName));
            }
        }
        public async Task<OperationResult> ZipFolderAsync(string uploadsRoot, string folderName)
        {
            try
            {
                var parentPath = Path.Combine(uploadsRoot, folderName);

                if (!Directory.Exists(parentPath))
                    return new OperationResult
                    {
                        Success = false,
                        Message = $"Folder không tồn tại: {folderName}"
                    };

                var subFolders = Directory.GetDirectories(parentPath);

                if (!subFolders.Any())
                    return new OperationResult
                    {
                        Success = false,
                        Message = $"Folder {folderName} không có thư mục con để zip"
                    };

                int zipCount = 0;
                long totalSize = 0;

                await Task.Run(() =>
                {
                    foreach (var dir in subFolders)
                    {
                        var groupName = Path.GetFileName(dir);
                        var zipPath = Path.Combine(parentPath, groupName + ".zip");

                        if (File.Exists(zipPath))
                            continue; // bỏ qua nếu đã zip rồi

                        using var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create);
                        AddDirectoryToZip(zip, dir, groupName);

                        var size = new FileInfo(zipPath).Length;
                        totalSize += size;
                        zipCount++;

                        // ❗ XÓA FOLDER GROUP SAU KHI ZIP
                        Directory.Delete(dir, true);
                    }
                });

                return new OperationResult
                {
                    Success = true,
                    Message = $"Đã zip {zipCount} group trong {folderName}",
                    Data = new
                    {
                        ParentFolder = folderName,
                        ZipCount = zipCount,
                        TotalZipSize = FormatSize(totalSize)
                    }
                };
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = $"Lỗi khi zip: {ex.Message}"
                };
            }
        }


        public async Task<OperationResult> UnzipArchiveAsync(
      string uploadsRoot,
      string parentFolder,
      string archiveFileName,
      bool deleteAfter = false)
        {
            try
            {
                if (!archiveFileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                    archiveFileName += ".zip";

                var parentPath = Path.Combine(uploadsRoot, parentFolder);
                var archivePath = Path.Combine(parentPath, archiveFileName);

                if (!Directory.Exists(parentPath))
                    return new OperationResult
                    {
                        Success = false,
                        Message = $"Folder không tồn tại: {parentFolder}"
                    };

                if (!File.Exists(archivePath))
                    return new OperationResult
                    {
                        Success = false,
                        Message = $"File không tồn tại: {parentFolder}/{archiveFileName}"
                    };

                var groupName = Path.GetFileNameWithoutExtension(archiveFileName);
                var finalPath = Path.Combine(parentPath, groupName);

                // Kiểm tra folder đích đã tồn tại chưa
                if (Directory.Exists(finalPath))
                    return new OperationResult
                    {
                        Success = false,
                        Message = $"Folder đã tồn tại: {parentFolder}/{groupName}"
                    };

                // Kiểm tra cấu trúc ZIP
                bool hasMatchingRootFolder = false;

                using (var zip = ZipFile.OpenRead(archivePath))
                {
                    var entries = zip.Entries.Where(e => !string.IsNullOrEmpty(e.FullName)).ToList();

                    if (entries.Any())
                    {
                        var firstEntry = entries.First().FullName;
                        var firstSegment = firstEntry.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries)[0];

                        // Kiểm tra: tất cả entries đều nằm trong cùng 1 root folder
                        // VÀ root folder đó trùng tên với groupName
                        if (entries.All(e =>
                        {
                            var segments = e.FullName.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                            return segments.Length > 0 && segments[0] == firstSegment;
                        }) && firstSegment.Equals(groupName, StringComparison.OrdinalIgnoreCase))
                        {
                            hasMatchingRootFolder = true;
                            Console.WriteLine($"📦 ZIP contains matching root folder: {firstSegment}");
                        }
                    }
                }

                if (hasMatchingRootFolder)
                {
                    // ZIP có root folder trùng tên → Extract vào temp, rồi move nội dung
                    var tempPath = Path.Combine(parentPath, $"_temp_{Guid.NewGuid().ToString("N").Substring(0, 8)}");

                    await Task.Run(() =>
                        ZipFile.ExtractToDirectory(archivePath, tempPath)
                    );

                    // Move folder con ra ngoài
                    var extractedFolder = Path.Combine(tempPath, groupName);
                    Directory.Move(extractedFolder, finalPath);

                    // Xóa temp folder
                    Directory.Delete(tempPath, false);

                    Console.WriteLine($"✅ Extracted and moved: {tempPath}/{groupName} → {finalPath}");
                }
                else
                {
                    // ZIP không có root folder hoặc tên khác → Extract trực tiếp
                    await Task.Run(() =>
                        ZipFile.ExtractToDirectory(archivePath, finalPath)
                    );

                    Console.WriteLine($"✅ Extracted directly to: {finalPath}");
                }

                if (deleteAfter)
                    File.Delete(archivePath);

                var folderSize = GetDirectorySize(finalPath);

                return new OperationResult
                {
                    Success = true,
                    Message =
                        $"Đã giải nén thành công: {parentFolder}/{groupName} " +
                        $"({FormatSize(folderSize)})" +
                        (deleteAfter ? " - Đã xóa file zip" : ""),
                    Data = new
                    {
                        ParentFolder = parentFolder,
                        Group = groupName,
                        ExtractPath = finalPath,
                        Size = folderSize,
                        ArchiveDeleted = deleteAfter
                    }
                };
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = $"Lỗi khi giải nén: {ex.Message}"
                };
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

                // Kiểm tra xem có file zip tương ứng không
                var zipFilePath = groupDir + ".zip";
                bool hasZip = File.Exists(zipFilePath);

                var groupInfo = new GroupInfo
                {
                    GroupName = groupName,
                    ParentFolder = semesterName, // Tên folder cha (semester)
                    HasZip = hasZip,
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
            allGroups = allGroups.OrderBy(g => g.GroupName).ToList();

            // Phân trang
            var totalCount = allGroups.Count;
            var items = allGroups
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

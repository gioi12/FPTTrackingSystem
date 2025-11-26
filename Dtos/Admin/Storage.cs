using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Admin
{
    public class SemesterStorageInfo
    {
        public string Name { get; set; }
        public bool HasZipFile { get; set; }
        public bool HasFolder { get; set; }
        public long FolderSize { get; set; } // bytes
        public long ZipSize { get; set; } // bytes
        public string FolderSizeFormatted => FormatSize(FolderSize);
        public string ZipSizeFormatted => FormatSize(ZipSize);
        public long TotalSize => FolderSize + ZipSize;
        public string TotalSizeFormatted => FormatSize(TotalSize);

        public static string FormatSize(long bytes)
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

    public class FileNode
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsDirectory { get; set; }
        public long Size { get; set; }
        public string SizeFormatted { get; set; }
        public DateTime LastModified { get; set; }
        public List<FileNode> Children { get; set; } = new List<FileNode>();
    }

    public class ZipRequest
    {
        public string FolderName { get; set; }
    }

    public class UnzipRequest
    {
        public string ArchiveFileName { get; set; } // "Fall25.zip"
        public bool DeleteArchiveAfter { get; set; } = false;
    }

    public class OperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
    public class GroupInfo
    {
        public string GroupName { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }
        public string SizeFormatted { get; set; }
        public int FileCount { get; set; }
        public int SubFolderCount { get; set; }
        public string PdfFilePath { get; set; } 
        public string PdfFileName { get; set; }
        public DateTime LastModified { get; set; }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}

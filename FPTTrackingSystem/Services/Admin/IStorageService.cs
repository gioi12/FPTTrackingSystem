using DataTranferObjects.Admin;

namespace FPTTrackingSystem.Services.Admin
{
    public interface IStorageService
    {
        List<SemesterStorageInfo> GetAllSemesters(string uploadsRoot);
        FileNode GetFolderTree(string uploadsRoot, string semesterName);

        Task<OperationResult> ZipFolderAsync(string uploadsRoot, string folderName);
        Task<OperationResult> UnzipArchiveAsync(string uploadsRoot, string archiveFileName, bool deleteAfter = false);
        Task<OperationResult> DeleteSemesterAsync(string uploadsRoot, string semesterName, bool deleteAll = false);
        long GetDirectorySize(string path);
        PagedResult<GroupInfo> GetGroupsBySemester(string uploadsRoot, string semesterName, int pageNumber = 1, int pageSize = 10);

    }
}

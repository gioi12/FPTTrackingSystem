using DataTranferObjects.Admin;
using FPTTrackingSystem.Services.Admin;
using FPTTrackingSystem.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPTTrackingSystem.Controllers.Admin
{
    [ApiController]
    [Route("api/v1/admin/storage")]
    //[Authorize(Roles = "Admin")] 
    public class StorageController : ControllerBase
    {
        private readonly IStorageService _storageService;
        private readonly IWebHostEnvironment _env;
        private readonly string _uploadsRoot;

        public StorageController(IStorageService storageService, IWebHostEnvironment env)
        {
            _storageService = storageService;
            _env = env;
            _uploadsRoot = Path.Combine(_env.WebRootPath, "uploads");
        }

        /// <summary>
        /// Lấy danh sách tất cả các kỳ học
        /// GET: api/admin/storage/semesters
        /// </summary>
        [HttpGet("semesters")]
        public IActionResult GetAllSemesters()
        {
            var semesters = _storageService.GetAllSemesters(_uploadsRoot);
            return Ok(ApiResponse<List<SemesterStorageInfo>>.Success(semesters, "Lấy danh sách kỳ học thành công."));
        }

        /// <summary>
        /// Zip một folder kỳ học
        /// POST: api/admin/storage/zip
        /// Body: { "folderName": "Fall25" }
        /// </summary>
        [HttpPost("zip")]
        public async Task<IActionResult> ZipFolder([FromBody] ZipRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FolderName))
                throw new System.ComponentModel.DataAnnotations.ValidationException("FolderName không được để trống");

            var result = await _storageService.ZipFolderAsync(_uploadsRoot, request.FolderName);

            if (!result.Success)
                throw new System.ComponentModel.DataAnnotations.ValidationException(result.Message);

            return Ok(ApiResponse<object>.Success(result.Data, result.Message));
        }

        /// <summary>
        /// Giải nén một file ZIP
        /// POST: api/admin/storage/unzip
        /// Body: { "archiveFileName": "Fall25.zip", "deleteArchiveAfter": false }
        /// </summary>
        [HttpPost("unzip")]
        public async Task<IActionResult> UnzipArchive([FromBody] UnzipRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ArchiveFileName))
                throw new System.ComponentModel.DataAnnotations.ValidationException("ArchiveFileName không được để trống");

            var result = await _storageService.UnzipArchiveAsync(_uploadsRoot,request.parentFolder, request.ArchiveFileName, request.DeleteArchiveAfter);

            if (!result.Success)
                throw new System.ComponentModel.DataAnnotations.ValidationException(result.Message);

            return Ok(ApiResponse<object>.Success(result.Data, result.Message));
        }

        /// <summary>
        /// Lấy danh sách các nhóm trong một kỳ học (có phân trang)
        /// GET: api/admin/groups/Fall25?pageNumber=1&pageSize=10
        /// </summary>
        [HttpGet("{semesterName}")]
        public IActionResult GetGroupsBySemester(
            string semesterName,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = _storageService.GetGroupsBySemester(_uploadsRoot, semesterName, pageNumber, pageSize);

            return Ok(ApiResponse<PagedResult<GroupInfo>>.Success(
                result,
                $"Lấy danh sách nhóm của {semesterName} thành công. Trang {result.PageNumber}/{result.TotalPages}."
            ));
        }

       
      
    }
}
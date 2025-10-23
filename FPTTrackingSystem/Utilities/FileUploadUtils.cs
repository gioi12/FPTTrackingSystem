namespace FPTTrackingSystem.Utilities
{
    public class FileUploadUtils
    {
        /// <summary>
        /// Uploads a file to /wwwroot/uploads/{year}/{folder}/
        /// </summary>
        /// <param name="file">File cần upload</param>
        /// <param name="type">1 = milestones, 2 = tasks, 3 = documents</param>
        /// <param name="env">IWebHostEnvironment (để lấy wwwroot)</param>
        /// <returns>Đường dẫn public để lưu vào DB, ví dụ: /uploads/2025/documents/abc.pdf</returns>
        public static async Task<string> UploadFileAsync(IFormFile file, int type, IWebHostEnvironment env)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File không hợp lệ.");

            // Xác định thư mục theo số
            string folderName = type switch
            {
                1 => "milestone items",
                2 => "tasks",
                3 => "groups",
                _ => "others"
            };

            // Lấy năm hiện tại
            string currentYear = DateTime.Now.Year.ToString();

            // Đường dẫn thư mục trong wwwroot
            string uploadPath = Path.Combine(env.WebRootPath, "uploads", currentYear, folderName);
            Directory.CreateDirectory(uploadPath); // Tạo thư mục nếu chưa có

            // Tạo tên file duy nhất
            string fileName = $"{Path.GetFileName(file.FileName)}";
            string filePath = Path.Combine(uploadPath, fileName);

            // Lưu file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Trả về đường dẫn public (để lưu DB hoặc hiển thị)
            string fileUrl = $"/uploads/{currentYear}/{folderName}/{fileName}";
            return fileUrl;
        }

        /// <summary>
        /// Lấy tên thực thể (entity) dựa trên type code.
        /// </summary>
        public static string GetEntityName(int type)
        {
            return type switch
            {
                1 => "Delivery_item",
                2 => "Task",
                3 => "Group",
                _ => "Unknown"
            };
        }
    }
}

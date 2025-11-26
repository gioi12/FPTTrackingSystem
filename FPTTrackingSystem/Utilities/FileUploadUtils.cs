using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Layout;
using iText.Layout.Element;


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
        public static async Task<string> UploadFileAsync(IFormFile file, int type, IWebHostEnvironment env,string semester,string group)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File không hợp lệ.");
            // replace space in semester
            semester = semester.Replace(" ", "");

            // Xác định thư mục theo số
            string folderName = type switch
            {
                1 => "milestones",
                2 => "tasks",
                3 => "documents",
                _ => "others"
            };

            // Đường dẫn thư mục trong wwwroot
            string uploadPath = Path.Combine(env.WebRootPath,"uploads",semester,group,folderName);
            Directory.CreateDirectory(uploadPath); // Tạo thư mục nếu chưa có

            // Tạo tên file duy nhất
            string fileName = $"{Path.GetFileName(file.FileName.Replace(" ", ""))}";
            string filePath = Path.Combine(uploadPath, fileName);

            // Lưu file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Trả về đường dẫn public (để lưu DB hoặc hiển thị)
            string fileUrl = $"/uploads/{semester}/{group}/{folderName}/{fileName}";
            await AppendToPdfSectionAsync(env, semester, group, folderName, fileUrl);
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
        private static string InsertIntoSection(string content, string section, string filePath)
        {
            string sectionHeader = section switch
            {
                "milestones" => "Milestones:",
                "tasks" => "Tasks:",
                "documents" => "Documents:",
                _ => "Others:"
            };

            int index = content.IndexOf(sectionHeader);
            if (index == -1)
                return content;

            int insertPos = content.IndexOf("\n", index) + 1;

            string before = content.Substring(0, insertPos);
            string after = content.Substring(insertPos);

            return before + "- " + filePath + "\n" + after;
        }
        private static async Task AppendToPdfSectionAsync(
      IWebHostEnvironment env,
      string semester,
      string group,
      string section,
      string uploadedFilePath)
        {
            string folder = Path.Combine(env.WebRootPath, "uploads", semester, group);
            Directory.CreateDirectory(folder);

            string pdfPath = Path.Combine(folder, semester+"_"+group+".pdf");
            string tempPath = Path.Combine(folder, $"summary_temp_{Guid.NewGuid()}.pdf");
            string content = "";

            try
            {
                // ĐỌC PDF CŨ (nếu có)
                if (File.Exists(pdfPath))
                {
                    var fileInfo = new FileInfo(pdfPath);
                    if (fileInfo.Length > 0)
                    {
                        try
                        {
                            using (var reader = new PdfReader(pdfPath))
                            using (var pdf = new PdfDocument(reader))
                            {
                                var strategy = new SimpleTextExtractionStrategy();
                                for (int i = 1; i <= pdf.GetNumberOfPages(); i++)
                                {
                                    content += PdfTextExtractor.GetTextFromPage(pdf.GetPage(i), strategy) + "\n";
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Nếu PDF cũ bị lỗi → tạo mới
                            Console.WriteLine($"PDF cũ bị lỗi, tạo mới: {ex.Message}");
                            content = "";
                        }
                    }
                }

                // Nếu không có content → tạo template mới
                if (string.IsNullOrWhiteSpace(content))
                {
                    content = $@"Semester : {semester} - Group : {group}

                    Milestones:

                    Tasks:

                    Documents:

                    Others:

                    ";
                }

                // Chèn đường dẫn vào section
                content = InsertIntoSection(content, section, uploadedFilePath);

                // GHI VÀO FILE TẠM
                using (var writer = new PdfWriter(tempPath))
                using (var outPdf = new PdfDocument(writer))
                using (var doc = new Document(outPdf))
                {
                    doc.Add(new Paragraph(content));
                }

                // Đợi đảm bảo file đã đóng
                await Task.Delay(100);

                if (File.Exists(pdfPath))
                {
                    File.Delete(pdfPath);
                }
                File.Move(tempPath, pdfPath);
            }
            catch (Exception ex)
            {
                // Dọn dẹp file tạm nếu có lỗi
                if (File.Exists(tempPath))
                {
                    try { File.Delete(tempPath); } catch { }
                }
                throw new Exception($"Lỗi khi xử lý PDF: {ex.Message}", ex);
            }
        }

    }
}

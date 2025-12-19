using Entities.Models;

namespace FPTTrackingSystem.Helper
{
    public static class MockData
    {
        // Summer 2025
        // Bắt đầu: Thứ Hai, 28/04/2025
        // Kết thúc: Chủ Nhật, 17/08/2025 (16 tuần)
        public static readonly Semester SemesterSummer2025 = new Semester
        {
            Name = "Summer 2025",
            IsActive = false,
            Description = "Summer semester for Capstone projects 2025",
            StartAt = new DateTime(2025, 4, 28),
            EndAt = new DateTime(2025, 8, 17)
        };

        // Fall 2025
        // Bắt đầu: Thứ Hai, 18/08/2025
        // Kết thúc: Chủ Nhật, 07/12/2025 (16 tuần)
        public static readonly Semester SemesterFall2025 = new Semester
        {
            Name = "Fall 2025",
            IsActive = true,
            Description = "Fall semester for Capstone projects 2025",
            StartAt = new DateTime(2025, 9, 8),
            EndAt = new DateTime(2025, 12, 28)
        };

        // Spring 2026
        // Bắt đầu: Thứ Hai, 05/01/2026
        // Kết thúc: Chủ Nhật, 26/04/2026
        public static readonly Semester SemesterSpring2026 = new Semester
        {
            Name = "Spring 2026",
            IsActive = false,
            Description = "Spring semester for Capstone projects 2026",
            StartAt = new DateTime(2026, 1, 5),
            EndAt = new DateTime(2026, 4, 26)
        };

        // Danh sách tất cả các semester
        public static readonly List<Semester> AllSemesters = new List<Semester>
        {
            SemesterSummer2025,
            SemesterFall2025,
            SemesterSpring2026
        };


                public static readonly List<MajorCategory> MajorCategories = new()
        {
            new MajorCategory
            {
                Code = "SEP490",
                Name = "SE Capstone Project",
                IsActive = true,
            },
            new MajorCategory
            {
                Code = "SAP490",
                Name = "SAP Interdisciplinary Capstone Project",
                IsActive = true,
            },

            // Graphic Design
            new MajorCategory
            {
                Code = "GDP492",
                Name = "Capstone Project Graphic Design - Animation",
                IsActive = true,
            },
            new MajorCategory
            {
                Code = "GDP493",
                Name = "Capstone Project Graphic Design - Interaction Design",
                IsActive = true,
            },
            new MajorCategory
            {
                Code = "GDP494",
                Name = "Capstone Project Graphic Design - Communication Design",
                IsActive = true,
            },
            new MajorCategory
            {
                Code = "GDP495",
                Name = "Capstone Project Graphic Design - Multimedia Communication Design",
                IsActive = true,
            },
            new MajorCategory
            {
                Code = "GDP491",
                Name = "Capstone Project Graphic Design",
                IsActive = true,
            },

            // AI / IA / IoT / IS
            new MajorCategory
            {
                Code = "AIP491",
                Name = "AI Capstone Project",
                IsActive = true,
            },
            new MajorCategory
            {
                Code = "IAP491",
                Name = "IA Graduation Project",
                IsActive = true,
            },
            new MajorCategory
            {
                Code = "IOP490",
                Name = "IoT Capstone Project",
                IsActive = true,
            },
            new MajorCategory
            {
                Code = "ISP490",
                Name = "IS Capstone Project",
                IsActive = true,
            },

            // Graduation Thesis
            new MajorCategory
            {
                Code = "GRF491",
                Name = "Graduation Thesis - Finance",
                IsActive = true,
            },
            new MajorCategory
            {
                Code = "GRH491",
                Name = "Graduation Thesis - Hotel Management",
                IsActive = true,
            },
            new MajorCategory
            {
                Code = "GRI491",
                Name = "Graduation Thesis - International Business",
                IsActive = true,
            },
            new MajorCategory
            {
                Code = "GRM491",
                Name = "Graduation Thesis - Marketing",
                IsActive = true,
            }
        };

        public static readonly List<Account> Accounts = new()
    {
        // Semester 1 students
        new Account { Username = "user1@fpt.edu.vn", Password = "123456", RoleId = 1, User = new User { RollNumber = "SE140001", Fullname = "Nguyen Van An", Dob = new DateOnly(2000,1,1), Gender = true, Mail = "gioidmhe171512@fpt.edu.vn", Phone="0909000001", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE"} },
        new Account { Username = "user2@fpt.edu.vn", Password = "123456", RoleId = 1, User = new User { RollNumber = "SE140002", Fullname = "Pham Tien Minh", Dob = new DateOnly(2000,2,1), Gender = true, Mail = "huongtthe172436@fpt.edu.vn", Phone="0909000002", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE"} },
        new Account { Username = "user3@fpt.edu.vn", Password = "123456", RoleId = 1, User = new User { RollNumber = "SE140003", Fullname = "Le Thi Hoa", Dob = new DateOnly(2000,3,1), Gender = true, Mail = "haildhe172452@fpt.edu.vn", Phone="0909000003", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE"} },
        new Account { Username = "user4@fpt.edu.vn", Password = "123456", RoleId = 1, User = new User { RollNumber = "SE140004", Fullname = "Tran Van Binh", Dob = new DateOnly(2000,4,1), Gender = true, Mail = "handghe170064@fpt.edu.vn", Phone="0909000004", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE"} },
        new Account { Username = "user5@fpt.edu.vn", Password = "123456", RoleId = 1, User = new User { RollNumber = "SE140005", Fullname = "Nguyen Thi Mai", Dob = new DateOnly(2000,5,1), Gender = true, Mail = "cuonghvhe176362@fpt.edu.vn", Phone="0909000005", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE"} },
        new Account { Username = "user6@fpt.edu.vn", Password = "123456", RoleId = 1, User = new User { RollNumber = "SE140006", Fullname = "Le Van Cuong", Dob = new DateOnly(2000,6,1), Gender = true, Mail = "user6@fpt.edu.vn", Phone="0909000006", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE"} },

        new Account { Username = "lampt2@gmail.com", Password = "123456", RoleId = 2, User = new User { RollNumber="ME01", Fullname="Mentor Phan Truong Lam", Dob=new DateOnly(1995,6,1), Gender=true, MajorId=1, CampusId=1, Mail="lampt2@gmail.com", Phone="0909123456", Address="Ha Noi", StatusId="ACTIVE"} },

        // Semester 2 students
        new Account { Username = "gioidmhe171512@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170001", Fullname="Doan Manh Gioi", Dob=new DateOnly(2000,2,14), Gender=true, Mail="gioidmhe171512@fpt.edu.vn", Phone="0909123456", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE"} },
        new Account { Username = "huongtthe172436@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170002", Fullname="TT Huong", Dob=new DateOnly(2001,6,10), Gender=true, Mail="huongtthe172436@fpt.edu.vn", Phone="0909888777", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Da Nang", StatusId="ACTIVE"} },
        new Account { Username = "haildhe172452@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170003", Fullname="Le Duy Hai", Dob=new DateOnly(2001,9,21), Gender=true, Mail="haildhe172452@fpt.edu.vn", Phone="0911222333", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ho Chi Minh", StatusId="ACTIVE"} },
        new Account { Username = "handghe170064@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170004", Fullname="Dinh Gia Han", Dob=new DateOnly(2001,4,5), Gender=true, Mail="handghe170064@fpt.edu.vn", Phone="0988777666", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE"} },
        new Account { Username = "cuonghvhe176362@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170005", Fullname="Ha Van Cuong", Dob=new DateOnly(2001,4,5), Gender=true, Mail="cuonghvhe176362@fpt.edu.vn", Phone="0988777666", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE"} },

        // Semester 2 students (Group 2)
        new Account { Username = "se170006@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170006", Fullname="Phung Thi Linh", Dob=new DateOnly(2001,1,1), Gender=true, Mail="gioidmhe171512@fpt.edu.vn", Phone="0909111106", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE"} },
        new Account { Username = "se170007@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170007", Fullname="Nguyen Huy Hoang", Dob=new DateOnly(2001,2,2), Gender=true, Mail="huongtthe172436@fpt.edu.vn", Phone="0909111107", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE"} },
        new Account { Username = "se170008@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170008", Fullname="Cao Manh Dat", Dob=new DateOnly(2001,3,3), Gender=true, Mail="haildhe172452@fpt.edu.vn", Phone="0909111108", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE"} },
        new Account { Username = "se170009@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170009", Fullname="Nguyen Manh Huy", Dob=new DateOnly(2001,4,4), Gender=true, Mail="handghe170064@fpt.edu.vn", Phone="0909111109", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE"} },
        new Account { Username = "se170010@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170010", Fullname="Nguyen Minh Quan", Dob=new DateOnly(2001,5,5), Gender=true, Mail="cuonghvhe176362@fpt.edu.vn", Phone="0909111110", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE"} },

        // Semester 2 students (Group 3)
        new Account { Username = "anhntse170011@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170011", Fullname="Nguyen Tuan Anh", Dob=new DateOnly(2001,6,12), Gender=true,  Mail="gioidmhe171512@fpt.edu.vn", Phone="0909111111", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi",   StatusId="ACTIVE"} },
        new Account { Username = "thuvtse170012@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170012", Fullname="Vu Thi Thu", Dob=new DateOnly(2001,7,8), Gender=false, Mail="huongtthe172436@fpt.edu.vn", Phone="0909111112", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi",   StatusId="ACTIVE"} },
        new Account { Username = "duongnmse170013@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170013", Fullname="Nguyen Minh Duong", Dob=new DateOnly(2001,3,19), Gender=true,  Mail="haildhe172452@fpt.edu.vn", Phone="0909111113", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Bac Ninh", StatusId="ACTIVE"} },
        new Account { Username = "linhptse170014@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170014", Fullname="Pham Thi Linh", Dob=new DateOnly(2001,11,2), Gender=false, Mail="handghe170064@fpt.edu.vn", Phone="0909111114", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Hai Phong",StatusId="ACTIVE"} },
        new Account { Username = "sondvse170015@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170015", Fullname="Do Van Son", Dob=new DateOnly(2001,1,27), Gender=true,  Mail="cuonghvhe176362@fpt.edu.vn", Phone="0909111115", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi",   StatusId="ACTIVE"} },
        new Account { Username = "minhndmentor@fpt.edu.vn", Password="123456", RoleId=2, User=new User { RollNumber="ME02", Fullname="Nguyen Duc Minh", Dob=new DateOnly(1994,3,12), Gender=true, Mail="huongtthe172436@fpt.edu.vn", Phone="0912000002", MajorId=1, CampusId=1, Address="Ha Noi", StatusId="ACTIVE"} },

        // Semester 2 students (Group 4)
        new Account { Username = "hoanglvse170016@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170016", Fullname="Le Viet Hoang", Dob=new DateOnly(2001,9,9), Gender=true,  Mail="hoanglvse170016@fpt.edu.vn", Phone="0909111116", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Da Nang",  StatusId="ACTIVE"} },
        new Account { Username = "trangntse170017@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170017", Fullname="Nguyen Thi Trang", Dob=new DateOnly(2002,2,14), Gender=false, Mail="trangntse170017@fpt.edu.vn", Phone="0909111117", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Da Nang",  StatusId="ACTIVE"} },
        new Account { Username = "khoanmse170018@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170018", Fullname="Mai Minh Khoan", Dob=new DateOnly(2001,5,30), Gender=true,  Mail="khoanmse170018@fpt.edu.vn", Phone="0909111118", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Quang Nam",StatusId="ACTIVE"} },
        new Account { Username = "vyltse170019@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170019", Fullname="Le Thi Thanh Vy", Dob=new DateOnly(2002,8,21), Gender=false, Mail="vyltse170019@fpt.edu.vn", Phone="0909111119", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Hue",     StatusId="ACTIVE"} },
        new Account { Username = "nhatpvse170020@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170020", Fullname="Pham Van Nhat", Dob=new DateOnly(2001,12,6), Gender=true, Mail="nhatpvse170020@fpt.edu.vn", Phone="0909111120", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Da Nang",  StatusId="ACTIVE"} },
        new Account { Username = "linhltmentor@fpt.edu.vn", Password="123456", RoleId=2, User=new User { RollNumber="ME03", Fullname="Le Thi Linh", Dob=new DateOnly(1993,7,20), Gender=false, Mail="linhltmentor@fpt.edu.vn", Phone="0912000003", MajorId=1, CampusId=1, Address="Da Nang", StatusId="ACTIVE"} },

        // Semester 2 students (Group 5)
        new Account { Username = "ngocptse170021@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170021", Fullname="Pham Thi Ngoc", Dob=new DateOnly(2002,3,3), Gender=false, Mail="ngocptse170021@fpt.edu.vn", Phone="0909111121", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ho Chi Minh", StatusId="ACTIVE"} },
        new Account { Username = "hieuntse170022@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170022", Fullname="Nguyen Trung Hieu", Dob=new DateOnly(2001,10,18), Gender=true, Mail="hieuntse170022@fpt.edu.vn", Phone="0909111122", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ho Chi Minh", StatusId="ACTIVE"} },
        new Account { Username = "phuongttse170023@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170023", Fullname="Tran Thi Phuong", Dob=new DateOnly(2002,6,25), Gender=false, Mail="phuongttse170023@fpt.edu.vn", Phone="0909111123", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Binh Duong", StatusId="ACTIVE"} },
        new Account { Username = "khanhdtse170024@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170024", Fullname="Do Tuan Khanh", Dob=new DateOnly(2001,4,11), Gender=true, Mail="khanhdtse170024@fpt.edu.vn", Phone="0909111124", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Dong Nai", StatusId="ACTIVE"} },
        new Account { Username = "myhnse170025@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170025", Fullname="Nguyen Hoang My", Dob=new DateOnly(2002,9,2), Gender=false, Mail="myhnse170025@fpt.edu.vn", Phone="0909111125", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ho Chi Minh", StatusId="ACTIVE"} },
        new Account { Username = "hoangtvmentor@fpt.edu.vn", Password="123456", RoleId=2, User=new User { RollNumber="ME04", Fullname="Tran Van Hoang", Dob=new DateOnly(1992,11,5), Gender=true, Mail="hoangtvmentor@fpt.edu.vn", Phone="0912000004", MajorId=1, CampusId=1, Address="Ho Chi Minh", StatusId="ACTIVE"} },

        // Semester 2 students (Group 6)
        new Account { Username = "quanghvse170026@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170026", Fullname="Ha Viet Quang", Dob=new DateOnly(2001,2,9), Gender=true, Mail="quanghvse170026@fpt.edu.vn", Phone="0909111126", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE"} },
        new Account { Username = "hanntse170027@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170027", Fullname="Nguyen Thi Han", Dob=new DateOnly(2002,1,16), Gender=false, Mail="hanntse170027@fpt.edu.vn", Phone="0909111127", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Nam Dinh", StatusId="ACTIVE"} },
        new Account { Username = "datcmse170028@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170028", Fullname="Cao Minh Dat", Dob=new DateOnly(2001,7,4), Gender=true, Mail="datcmse170028@fpt.edu.vn", Phone="0909111128", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Thai Nguyen", StatusId="ACTIVE"} },
        new Account { Username = "yenbtse170029@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170029", Fullname="Bui Thi Yen", Dob=new DateOnly(2002,12,12), Gender=false, Mail="yenbtse170029@fpt.edu.vn", Phone="0909111129", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE"} },
        new Account { Username = "kientvse170030@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170030", Fullname="Tran Van Kien", Dob=new DateOnly(2001,8,28), Gender=true, Mail="kientvse170030@fpt.edu.vn", Phone="0909111130", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE"} },
        new Account { Username = "huongptmentor@fpt.edu.vn", Password="123456", RoleId=2, User=new User { RollNumber="ME05", Fullname="Pham Thu Huong", Dob=new DateOnly(1994,2,28), Gender=false, Mail="huongptmentor@fpt.edu.vn", Phone="0912000005", MajorId=1, CampusId=1, Address="Ha Noi", StatusId="ACTIVE"} },

        // Semester 2 students (Group 7)
        new Account { Username = "ductmse170031@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170031", Fullname="Tran Minh Duc", Dob=new DateOnly(2001,3,15), Gender=true, Mail="ductmse170031@fpt.edu.vn", Phone="0909111131", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Da Nang", StatusId="ACTIVE"} },
        new Account { Username = "nganptse170032@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170032", Fullname="Phan Thi Ngan", Dob=new DateOnly(2002,5,20), Gender=false, Mail="nganptse170032@fpt.edu.vn", Phone="0909111132", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Quang Ngai", StatusId="ACTIVE"} },
        new Account { Username = "thanhddse170033@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170033", Fullname="Do Duy Thanh", Dob=new DateOnly(2001,11,11), Gender=true, Mail="thanhddse170033@fpt.edu.vn", Phone="0909111133", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Da Nang", StatusId="ACTIVE"} },
        new Account { Username = "anhltse170034@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170034", Fullname="Le Thi Anh", Dob=new DateOnly(2002,7,7), Gender=false, Mail="anhltse170034@fpt.edu.vn", Phone="0909111134", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Hue", StatusId="ACTIVE"} },
        new Account { Username = "vudkse170035@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170035", Fullname="Dang Khac Vu", Dob=new DateOnly(2001,9,30), Gender=true, Mail="vudkse170035@fpt.edu.vn", Phone="0909111135", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Da Nang", StatusId="ACTIVE"} },
        new Account { Username = "quanghmmentor@fpt.edu.vn", Password="123456", RoleId=2, User=new User { RollNumber="ME06", Fullname="Hoang Minh Quang", Dob=new DateOnly(1991,9,14), Gender=true, Mail="quanghmmentor@fpt.edu.vn", Phone="0912000006", MajorId=1, CampusId=1, Address="Da Nang", StatusId="ACTIVE"} },

        // FALL 2025 – GROUP 08
        new Account { Username="datqtse180036@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE180036", Fullname="Tran Quoc Dat", Dob=new DateOnly(2001,1,1), Gender=true, Mail="gioidmhe171512@fpt.edu.vn", Phone="0909111136", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE" } },
        new Account { Username="lanntse180037@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE180037", Fullname="Nguyen Thi Lan", Dob=new DateOnly(2001,2,2), Gender=false, Mail="huongtthe172436@fpt.edu.vn", Phone="0909111137", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE" } },
        new Account { Username="quanlmse180038@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE180038", Fullname="Le Minh Quan", Dob=new DateOnly(2001,3,3), Gender=true, Mail="haildhe172452@fpt.edu.vn", Phone="0909111138", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE" } },
        new Account { Username="trangptse180039@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE180039", Fullname="Pham Thu Trang", Dob=new DateOnly(2001,4,4), Gender=false, Mail="handghe170064@fpt.edu.vn", Phone="0909111139", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE" } },
        new Account { Username="hungdvse180040@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE180040", Fullname="Do Van Hung", Dob=new DateOnly(2001,5,5), Gender=true, Mail="cuonghvhe176362@fpt.edu.vn", Phone="0909111140", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE" } },
        new Account { Username="ngavme07@fpt.edu.vn", Password="123456", RoleId=2, User=new User { RollNumber="ME07", Fullname="Nguyen Van A", Dob=new DateOnly(1989,1,1), Gender=true, Mail="gioidmhe171512@fpt.edu.vn", Phone="0912000007", MajorId=1, CampusId=1, Address="Ha Noi", StatusId="ACTIVE" } },

        // FALL 2025 – GROUP 09
        new Account { Username="ducnmse180041@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE180041", Fullname="Nguyen Minh Duc", Dob=new DateOnly(2001,6,6), Gender=true, Mail="gioidmhe171512@fpt.edu.vn", Phone="0909111141", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE" } },
        new Account { Username="anhpnse180042@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE180042", Fullname="Pham Ngoc Anh", Dob=new DateOnly(2001,7,7), Gender=false, Mail="huongtthe172436@fpt.edu.vn", Phone="0909111142", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE" } },
        new Account { Username="namlvse180043@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE180043", Fullname="Le Van Nam", Dob=new DateOnly(2001,8,8), Gender=true, Mail="haildhe172452@fpt.edu.vn", Phone="0909111143", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE" } },
        new Account { Username="bichttse180044@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE180044", Fullname="Tran Thi Bich", Dob=new DateOnly(2001,9,9), Gender=false, Mail="handghe170064@fpt.edu.vn", Phone="0909111144", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE" } },
        new Account { Username="tuandvse180045@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE180045", Fullname="Do Anh Tuan", Dob=new DateOnly(2001,10,10), Gender=true, Mail="cuonghvhe176362@fpt.edu.vn", Phone="0909111145", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE" } },
        new Account { Username="tranbvme08@fpt.edu.vn", Password="123456", RoleId=2, User=new User { RollNumber="ME08", Fullname="Tran Van B", Dob=new DateOnly(1988,2,2), Gender=true, Mail="huongtthe172436@fpt.edu.vn", Phone="0912000008", MajorId=1, CampusId=1, Address="Ha Noi", StatusId="ACTIVE" } },

        // FALL 2025 – GROUP 10
        new Account { Username="longnvse180046@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE180046", Fullname="Nguyen Van Long", Dob=new DateOnly(2001,11,11), Gender=true, Mail="gioidmhe171512@fpt.edu.vn", Phone="0909111146", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE" } },
        new Account { Username="haptse180047@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE180047", Fullname="Pham Thi Ha", Dob=new DateOnly(2001,12,12), Gender=false, Mail="huongtthe172436@fpt.edu.vn", Phone="0909111147", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE" } },
        new Account { Username="hoanglmse180048@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE180048", Fullname="Le Minh Hoang", Dob=new DateOnly(2001,3,13), Gender=true, Mail="haildhe172452@fpt.edu.vn", Phone="0909111148", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE" } },
        new Account { Username="maitnse180049@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE180049", Fullname="Tran Ngoc Mai", Dob=new DateOnly(2001,4,14), Gender=false, Mail="handghe170064@fpt.edu.vn", Phone="0909111149", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE" } },
        new Account { Username="dungdtse180050@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE180050", Fullname="Do Tien Dung", Dob=new DateOnly(2001,5,15), Gender=true, Mail="cuonghvhe176362@fpt.edu.vn", Phone="0909111150", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi", StatusId="ACTIVE" } },
        new Account { Username="levcme09@fpt.edu.vn", Password="123456", RoleId=2, User=new User { RollNumber="ME09", Fullname="Le Van C", Dob=new DateOnly(1987,3,3), Gender=true, Mail="haildhe172452@fpt.edu.vn", Phone="0912000009", MajorId=1, CampusId=1, Address="Ha Noi", StatusId="ACTIVE" } },


        // Spring 2026
        //G01
        new Account { Username="khanhdtse190051@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190051", Fullname="Do Tuan Khanh", Dob=new DateOnly(2002,11,3), Gender=true, Mail="gioidmhe171512@fpt.edu.vn", Phone="0909333051", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="myhnse190052@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190052", Fullname="Nguyen Hoang My", Dob=new DateOnly(2002,12,9), Gender=false, Mail="huongtthe172436@fpt.edu.vn", Phone="0909333052", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="phucnvse190053@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190053", Fullname="Nguyen Van Phuc", Dob=new DateOnly(2002,1,6), Gender=true, Mail="haildhe172452@fpt.edu.vn", Phone="0909333053", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="trangltse190054@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190054", Fullname="Le Thi Trang", Dob=new DateOnly(2002,2,14), Gender=false, Mail="handghe170064@fpt.edu.vn", Phone="0909333054", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="quocbmse190055@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190055", Fullname="Bui Minh Quoc", Dob=new DateOnly(2002,3,22), Gender=true, Mail="cuonghvhe176362@fpt.edu.vn", Phone="0909333055", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="anhdume10@fpt.edu.vn", Password="123456", RoleId=2, User=new User{ RollNumber="ME10", Fullname="Nguyen Duc Anh", Dob=new DateOnly(1984,3,12), Gender=true, Mail="anhnd@fpt.edu.vn", Phone="0912111110", MajorId=1, CampusId=1, Address="Ha Noi", StatusId="ACTIVE"} },
        //G02
        new Account { Username="linhntse190056@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190056", Fullname="Nguyen Thi Linh", Dob=new DateOnly(2002,4,18), Gender=false, Mail="gioidmhe171512@fpt.edu.vn", Phone="0909333056", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="hoangnvse190057@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190057", Fullname="Nguyen Van Hoang", Dob=new DateOnly(2002,5,27), Gender=true, Mail="huongtthe172436@fpt.edu.vn", Phone="0909333057", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="anhptse190058@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190058", Fullname="Pham Thi Anh", Dob=new DateOnly(2002,6,8), Gender=false, Mail="haildhe172452@fpt.edu.vn", Phone="0909333058", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="duynmse190059@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190059", Fullname="Nguyen Minh Duy", Dob=new DateOnly(2002,7,19), Gender=true, Mail="handghe170064@fpt.edu.vn", Phone="0909333059", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="vyltse190060@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190060", Fullname="Le Thi Thanh Vy", Dob=new DateOnly(2002,8,26), Gender=false, Mail="cuonghvhe176362@fpt.edu.vn", Phone="0909333060", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="hoanglvme11@fpt.edu.vn", Password="123456", RoleId=2, User=new User{ RollNumber="ME11", Fullname="Le Van Hoang", Dob=new DateOnly(1983,7,18), Gender=true, Mail="hoanglv@fpt.edu.vn", Phone="0912111111", MajorId=1, CampusId=1, Address="Ha Noi", StatusId="ACTIVE"} },
        //G03
        new Account { Username="datqtse190011@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190011", Fullname="Tran Quoc Dat", Dob=new DateOnly(2002,1,11), Gender=true, Mail="gioidmhe171512@fpt.edu.vn", Phone="0909333011", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="lanntse190012@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190012", Fullname="Nguyen Thi Lan", Dob=new DateOnly(2002,2,12), Gender=false, Mail="huongtthe172436@fpt.edu.vn", Phone="0909333012", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="quanlmse190013@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190013", Fullname="Le Minh Quan", Dob=new DateOnly(2002,3,13), Gender=true, Mail="haildhe172452@fpt.edu.vn", Phone="0909333013", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="trangptse190014@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190014", Fullname="Pham Thu Trang", Dob=new DateOnly(2002,4,14), Gender=false, Mail="handghe170064@fpt.edu.vn", Phone="0909333014", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="hungdvse190015@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190015", Fullname="Do Van Hung", Dob=new DateOnly(2002,5,15), Gender=true, Mail="cuonghvhe176362@fpt.edu.vn", Phone="0909333015", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="thuptme12@fpt.edu.vn", Password="123456", RoleId=2, User=new User{ RollNumber="ME12", Fullname="Pham Thi Thu", Dob=new DateOnly(1986,1,25), Gender=false, Mail="thupt@fpt.edu.vn", Phone="0912111112", MajorId=1, CampusId=1, Address="Hai Phong", StatusId="ACTIVE"} },
        //G04
        new Account { Username="ducnmse190016@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190016", Fullname="Nguyen Minh Duc", Dob=new DateOnly(2002,6,16), Gender=true, Mail="gioidmhe171512@fpt.edu.vn", Phone="0909333016", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="anhpnse190017@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190017", Fullname="Pham Ngoc Anh", Dob=new DateOnly(2002,7,17), Gender=false, Mail="huongtthe172436@fpt.edu.vn", Phone="0909333017", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="namlvse190018@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190018", Fullname="Le Van Nam", Dob=new DateOnly(2002,8,18), Gender=true, Mail="haildhe172452@fpt.edu.vn", Phone="0909333018", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="bichttse190019@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190019", Fullname="Tran Thi Bich", Dob=new DateOnly(2002,9,19), Gender=false, Mail="handghe170064@fpt.edu.vn", Phone="0909333019", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="tuandvse190020@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190020", Fullname="Do Anh Tuan", Dob=new DateOnly(2002,10,20), Gender=true, Mail="cuonghvhe176362@fpt.edu.vn", Phone="0909333020", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="minhntme13@fpt.edu.vn", Password="123456", RoleId=2, User=new User{ RollNumber="ME13", Fullname="Nguyen Thanh Minh", Dob=new DateOnly(1982,11,9), Gender=true, Mail="minhnt@fpt.edu.vn", Phone="0912111113", MajorId=1, CampusId=1, Address="Da Nang", StatusId="ACTIVE"} },
        //G05
        new Account { Username="datqtse190021@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190021", Fullname="Tran Quoc Dat", Dob=new DateOnly(2002,1,21), Gender=true, Mail="gioidmhe171512@fpt.edu.vn", Phone="0909333021", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="lanntse190022@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190022", Fullname="Nguyen Thi Lan", Dob=new DateOnly(2002,2,22), Gender=false, Mail="huongtthe172436@fpt.edu.vn", Phone="0909333022", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="quanlmse190023@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190023", Fullname="Le Minh Quan", Dob=new DateOnly(2002,3,23), Gender=true, Mail="haildhe172452@fpt.edu.vn", Phone="0909333023", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="trangptse190024@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190024", Fullname="Pham Thu Trang", Dob=new DateOnly(2002,4,24), Gender=false, Mail="handghe170064@fpt.edu.vn", Phone="0909333024", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="hungdvse190025@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190025", Fullname="Do Van Hung", Dob=new DateOnly(2002,5,25), Gender=true, Mail="cuonghvhe176362@fpt.edu.vn", Phone="0909333025", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
         new Account { Username="linhptme14@fpt.edu.vn", Password="123456", RoleId=2, User=new User{ RollNumber="ME14", Fullname="Pham Thi Linh", Dob=new DateOnly(1987,6,2), Gender=false, Mail="linhpt@fpt.edu.vn", Phone="0912111114", MajorId=1, CampusId=1, Address="Ha Noi", StatusId="ACTIVE"} },
        //G06
        new Account { Username="ducnmse190026@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190026", Fullname="Nguyen Minh Duc", Dob=new DateOnly(2002,6,26), Gender=true, Mail="gioidmhe171512@fpt.edu.vn", Phone="0909333026", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="anhpnse190027@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190027", Fullname="Pham Ngoc Anh", Dob=new DateOnly(2002,7,27), Gender=false, Mail="huongtthe172436@fpt.edu.vn", Phone="0909333027", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="namlvse190028@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190028", Fullname="Le Van Nam", Dob=new DateOnly(2002,8,28), Gender=true, Mail="haildhe172452@fpt.edu.vn", Phone="0909333028", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="bichttse190029@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190029", Fullname="Tran Thi Bich", Dob=new DateOnly(2002,9,29), Gender=false, Mail="handghe170064@fpt.edu.vn", Phone="0909333029", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="tuandvse190030@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190030", Fullname="Do Anh Tuan", Dob=new DateOnly(2002,10,30), Gender=true, Mail="cuonghvhe176362@fpt.edu.vn", Phone="0909333030", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="quanghmme15@fpt.edu.vn", Password="123456", RoleId=2, User=new User{ RollNumber="ME15", Fullname="Hoang Minh Quang", Dob=new DateOnly(1981,9,14), Gender=true, Mail="quanghm@fpt.edu.vn", Phone="0912111115", MajorId=1, CampusId=1, Address="Da Nang", StatusId="ACTIVE"} },
        //G07
        new Account { Username="datqtse190031@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190031", Fullname="Tran Quoc Dat", Dob=new DateOnly(2002,1,31), Gender=true, Mail="gioidmhe171512@fpt.edu.vn", Phone="0909333031", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="lanntse190032@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190032", Fullname="Nguyen Thi Lan", Dob=new DateOnly(2002,2,15), Gender=false, Mail="huongtthe172436@fpt.edu.vn", Phone="0909333032", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="quanlmse190033@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190033", Fullname="Le Minh Quan", Dob=new DateOnly(2002,3,16), Gender=true, Mail="haildhe172452@fpt.edu.vn", Phone="0909333033", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="trangptse190034@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190034", Fullname="Pham Thu Trang", Dob=new DateOnly(2002,4,17), Gender=false, Mail="handghe170064@fpt.edu.vn", Phone="0909333034", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="hungdvse190035@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190035", Fullname="Do Van Hung", Dob=new DateOnly(2002,5,18), Gender=true, Mail="cuonghvhe176362@fpt.edu.vn", Phone="0909333035", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="ngocttme16@fpt.edu.vn", Password="123456", RoleId=2, User=new User{ RollNumber="ME16", Fullname="Tran Thi Ngoc", Dob=new DateOnly(1985,4,30), Gender=false, Mail="ngoctt@fpt.edu.vn", Phone="0912111116", MajorId=1, CampusId=1, Address="Ho Chi Minh", StatusId="ACTIVE"} },
        //G08
        new Account { Username="ducnmse190036@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190036", Fullname="Nguyen Minh Duc", Dob=new DateOnly(2002,6,19), Gender=true, Mail="gioidmhe171512@fpt.edu.vn", Phone="0909333036", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="anhpnse190037@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190037", Fullname="Pham Ngoc Anh", Dob=new DateOnly(2002,7,20), Gender=false, Mail="huongtthe172436@fpt.edu.vn", Phone="0909333037", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="namlvse190038@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190038", Fullname="Le Van Nam", Dob=new DateOnly(2002,8,21), Gender=true, Mail="haildhe172452@fpt.edu.vn", Phone="0909333038", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="bichttse190039@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190039", Fullname="Tran Thi Bich", Dob=new DateOnly(2002,9,22), Gender=false, Mail="handghe170064@fpt.edu.vn", Phone="0909333039", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="tuandvse190040@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190040", Fullname="Do Anh Tuan", Dob=new DateOnly(2002,10,23), Gender=true, Mail="cuonghvhe176362@fpt.edu.vn", Phone="0909333040", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="duongnvme17@fpt.edu.vn", Password="123456", RoleId=2, User=new User{ RollNumber="ME17", Fullname="Nguyen Van Duong", Dob=new DateOnly(1980,8,21), Gender=true, Mail="duongnv@fpt.edu.vn", Phone="0912111117", MajorId=1, CampusId=1, Address="Bac Ninh", StatusId="ACTIVE"} },
        //G09
        new Account { Username="datqtse190041@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190041", Fullname="Tran Quoc Dat", Dob=new DateOnly(2002,1,24), Gender=true, Mail="gioidmhe171512@fpt.edu.vn", Phone="0909333041", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="lanntse190042@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190042", Fullname="Nguyen Thi Lan", Dob=new DateOnly(2002,2,25), Gender=false, Mail="huongtthe172436@fpt.edu.vn", Phone="0909333042", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="quanlmse190043@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190043", Fullname="Le Minh Quan", Dob=new DateOnly(2002,3,26), Gender=true, Mail="haildhe172452@fpt.edu.vn", Phone="0909333043", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="trangptse190044@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190044", Fullname="Pham Thu Trang", Dob=new DateOnly(2002,4,27), Gender=false, Mail="handghe170064@fpt.edu.vn", Phone="0909333044", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="hungdvse190045@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190045", Fullname="Do Van Hung", Dob=new DateOnly(2002,5,28), Gender=true, Mail="cuonghvhe176362@fpt.edu.vn", Phone="0909333045", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="huongntme18@fpt.edu.vn", Password="123456", RoleId=2, User=new User{ RollNumber="ME18", Fullname="Nguyen Thi Huong", Dob=new DateOnly(1988,12,5), Gender=false, Mail="huongnt@fpt.edu.vn", Phone="0912111118", MajorId=1, CampusId=1, Address="Nam Dinh", StatusId="ACTIVE"} },
        //G10
        new Account { Username="ducnmse190046@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190046", Fullname="Nguyen Minh Duc", Dob=new DateOnly(2002,6,29), Gender=true, Mail="gioidmhe171512@fpt.edu.vn", Phone="0909333046", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="anhpnse190047@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190047", Fullname="Pham Ngoc Anh", Dob=new DateOnly(2002,7,30), Gender=false, Mail="huongtthe172436@fpt.edu.vn", Phone="0909333047", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="namlvse190048@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190048", Fullname="Le Van Nam", Dob=new DateOnly(2002,8,31), Gender=true, Mail="haildhe172452@fpt.edu.vn", Phone="0909333048", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="bichttse190049@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190049", Fullname="Tran Thi Bich", Dob=new DateOnly(2002,9,1), Gender=false, Mail="handghe170064@fpt.edu.vn", Phone="0909333049", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="tuandvse190050@fpt.edu.vn", Password="123456", RoleId=1, User=new User{ RollNumber="SE190050", Fullname="Do Anh Tuan", Dob=new DateOnly(2002,10,2), Gender=true, Mail="cuonghvhe176362@fpt.edu.vn", Phone="0909333050", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="VN", StatusId="ACTIVE"} },
        new Account { Username="sondvme19@fpt.edu.vn", Password="123456", RoleId=2, User=new User{ RollNumber="ME19", Fullname="Do Van Son", Dob=new DateOnly(1983,2,17), Gender=true, Mail="sondv@fpt.edu.vn", Phone="0912111119", MajorId=1, CampusId=1, Address="Ha Noi", StatusId="ACTIVE"} },
    };

        public static List<Group> GetGroupsForSemester(int semesterId, string semesterName)
        {
            // =========================
            // SUMMER 2025
            // =========================
            if (semesterName.Equals("Summer 2025"))
            {
                var students = Accounts
                    .Where(a => a.RoleId == 1 && a.User.RollNumber.StartsWith("SE140"))
                    .ToList();

                var mentor = Accounts.FirstOrDefault(a => a.RoleId == 2);
                if (mentor == null)
                    throw new Exception("Không tìm thấy mentor cho Summer 2025");

                return new List<Group>
        {
            new Group
            {
                Code = "G01",
                Name = "Capstone Team Vanguard",
                SemesterId = semesterId,
                CreateAt = DateTime.Now.AddMonths(-2),
                Profession = "AI Development",
                MajorId = 1,
                Description = "Team phát triển hệ thống AI",
                VietnameseTitle = "Nhóm Tiên Phong",
                StatusId = "ACTIVE",
                MeetingId = 1,
                ExpireDate = DateTime.Now.AddMonths(6),
                GroupUsers = students.Select((s, i) => new GroupUser
                {
                    User = s.User,
                    Role = i == 0 ? "Leader" : "Student",
                    IsActive = true,
                    CreateAt = DateTime.Now.AddMonths(-2),
                    UpdateAt = DateTime.Now,
                    Status = "Active"
                })
                .Concat(new[]
                {
                    new GroupUser
                    {
                        User = mentor.User,
                        Role = "Supervisor",
                        IsActive = true,
                        CreateAt = DateTime.Now.AddMonths(-1),
                        UpdateAt = DateTime.Now,
                        Status = "Active"
                    }
                }).ToList()
            }
        };
            }

            // =========================
            // FALL 2025 (G11 – G12)
            // =========================
            if (semesterName.Equals("Fall 2025"))
            {
                var allStudents = Accounts
                                .Where(a => a.RoleId == 1 &&
                                    (a.User.RollNumber.StartsWith("SE170")
                                  || a.User.RollNumber.StartsWith("SE180")
                                  || a.User.RollNumber.StartsWith("SE190")))
                                .ToList();
                var mentors = Accounts
                    .Where(a => a.RoleId == 2)
                    .ToDictionary(a => a.User.RollNumber);

                List<Account> GetStudents(int from, int to) =>
                    allStudents.Where(a =>
                        int.Parse(a.User.RollNumber.Substring(6)) >= from &&
                        int.Parse(a.User.RollNumber.Substring(6)) <= to
                    ).ToList();

                Group CreateGroup(
                    string code,
                    string name,
                    string vnTitle,
                    string profession,
                    List<Account> students,
                    string mentorRoll
                )
                {
                    if (!mentors.ContainsKey(mentorRoll))
                        throw new Exception($"Không tìm thấy mentor {mentorRoll}");

                    var mentor = mentors[mentorRoll];

                    return new Group
                    {
                        Code = code,
                        Name = name,
                        SemesterId = semesterId,
                        CreateAt = DateTime.Now.AddMonths(-2),
                        Profession = profession,
                        MajorId = 1,
                        Description = $"{name} Capstone Project",
                        VietnameseTitle = vnTitle,
                        StatusId = "ACTIVE",
                        ExpireDate = DateTime.Now.AddMonths(6),
                        GroupUsers = students.Select((s, i) => new GroupUser
                        {
                            User = s.User,
                            Role = i == 0 ? "Leader" : "Student",
                            IsActive = true,
                            CreateAt = DateTime.Now.AddMonths(-2),
                            UpdateAt = DateTime.Now,
                            Status = "Active"
                        })
                        .Concat(new[]
                        {
                    new GroupUser
                    {
                        User = mentor.User,
                        Role = "Supervisor",
                        IsActive = true,
                        CreateAt = DateTime.Now.AddMonths(-1),
                        UpdateAt = DateTime.Now,
                        Status = "Active"
                    }
                        }).ToList()
                    };
                }

                return new List<Group>
        {
            CreateGroup(
                "G01",
                "FPT Tracking System",
                "Hệ thống theo dõi FPT",
                "Software Engineering",
                GetStudents(1, 5),
                "ME01"
            ),

            CreateGroup(
                "G02",
                "Smart Course Booking",
                "Hệ thống đăng ký môn học thông minh",
                "Software Engineering",
                GetStudents(6, 10),
                "ME02"
            ),
            CreateGroup("G03", "AI Student Progress Monitor", "Theo dõi tiến độ sinh viên bằng AI", "AI Development", GetStudents(11, 15), "ME03"),
            CreateGroup("G04", "Campus Event Management System", "Quản lý sự kiện trong campus", "Software Engineering", GetStudents(16, 20), "ME04"),
            CreateGroup("G05", "Internship Matching Platform", "Nền tảng kết nối thực tập", "Software Engineering", GetStudents(21, 25), "ME05"),
            CreateGroup("G06", "Smart Attendance System", "Hệ thống điểm danh thông minh", "Software Engineering", GetStudents(26, 30), "ME06"),
            CreateGroup("G07", "AI Career Recommendation", "Gợi ý nghề nghiệp bằng AI", "AI Development", GetStudents(31, 35), "ME07"),
            CreateGroup("G08", "Digital Attendance AI", "Điểm danh AI", "AI Development", GetStudents(36, 40), "ME07"),
            CreateGroup("G09", "Smart Dormitory System", "Quản lý ký túc xá", "Software Engineering", GetStudents(41, 45), "ME08"),
            CreateGroup("G10", "Student Feedback Platform", "Phản hồi sinh viên", "Software Engineering", GetStudents(46, 50), "ME09"),
        };
            }

            // =========================
            // SPRING 2026 (G13 – G17)
            // =========================
            if (semesterName.Equals("Spring 2026"))
            {
                var allStudents = Accounts
                                 .Where(a => a.RoleId == 1 &&
                                     (a.User.RollNumber.StartsWith("SE190")))
                                 .ToList();


                var mentors = Accounts
                    .Where(a => a.RoleId == 2)
                    .ToDictionary(a => a.User.RollNumber);

                List<Account> GetStudents(int from, int to) =>
                    allStudents.Where(a =>
                        int.Parse(a.User.RollNumber.Substring(6)) >= from &&
                        int.Parse(a.User.RollNumber.Substring(6)) <= to
                    ).ToList();

                Group CreateGroup(
                    string code,
                    string name,
                    string vnTitle,
                    string profession,
                    List<Account> students,
                    string mentorRoll
                )
                {
                    if (!mentors.ContainsKey(mentorRoll))
                        throw new Exception($"Không tìm thấy mentor {mentorRoll}");

                    var mentor = mentors[mentorRoll];

                    return new Group
                    {
                        Code = code,
                        Name = name,
                        SemesterId = semesterId,
                        CreateAt = DateTime.Now.AddMonths(-2),
                        Profession = profession,
                        MajorId = 1,
                        Description = $"{name} Capstone Project",
                        VietnameseTitle = vnTitle,
                        StatusId = "ACTIVE",
                        ExpireDate = DateTime.Now.AddMonths(6),
                        GroupUsers = students.Select((s, i) => new GroupUser
                        {
                            User = s.User,
                            Role = i == 0 ? "Leader" : "Student",
                            IsActive = true,
                            CreateAt = DateTime.Now.AddMonths(-2),
                            UpdateAt = DateTime.Now,
                            Status = "Active"
                        })
                        .Concat(new[]
                        {
                    new GroupUser
                    {
                        User = mentor.User,
                        Role = "Supervisor",
                        IsActive = true,
                        CreateAt = DateTime.Now.AddMonths(-1),
                        UpdateAt = DateTime.Now,
                        Status = "Active"
                    }
                        }).ToList()
                    };
                }

                return new List<Group>
                {
                    CreateGroup("G01","AI Study Planner","Lập kế hoạch học AI","AI Development",GetStudents(51,55),"ME10"),
                    CreateGroup("G02","Smart Library","Thư viện thông minh","Software Engineering",GetStudents(56,60),"ME11"),
                    CreateGroup("G03","Exam Proctoring AI","Giám sát thi AI","AI Development",GetStudents(11,15),"ME12"),
                    CreateGroup("G04","Intern Tracker","Theo dõi thực tập","Software Engineering",GetStudents(16,20),"ME13"),
                    CreateGroup("G05","Campus Chatbot","Chatbot campus","AI Development",GetStudents(21,25),"ME14"),
                    CreateGroup("G06","Learning Analytics","Phân tích học tập","AI Development",GetStudents(26,30),"ME15"),
                    CreateGroup("G07","Course Recommendation","Gợi ý môn học","AI Development",GetStudents(31,35),"ME16"),
                    CreateGroup("G08","Online Defense System","Bảo vệ online","Software Engineering",GetStudents(36,40),"ME17"),
                    CreateGroup("G09","Student CRM","CRM sinh viên","Software Engineering",GetStudents(41,45),"ME18"),
                    CreateGroup("G10","Thesis Management","Quản lý khóa luận","Software Engineering",GetStudents(46,50),"ME19"),
                };
            }

            return new List<Group>();
        }


    }
}
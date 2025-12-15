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
                Name = "Software Engineering Project",
                IsActive = true,
            },
            new MajorCategory
            {
                Code = "SAP490",
                Name = "Strategic Advertising Project",
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
        new Account { Username = "anhntse170011@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170011", Fullname="Nguyen Tuan Anh", Dob=new DateOnly(2001,6,12), Gender=true,  Mail="anhntse170011@fpt.edu.vn", Phone="0909111111", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi",   StatusId="ACTIVE"} },
        new Account { Username = "thuvtse170012@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170012", Fullname="Vu Thi Thu", Dob=new DateOnly(2001,7,8), Gender=false, Mail="thuvtse170012@fpt.edu.vn", Phone="0909111112", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi",   StatusId="ACTIVE"} },
        new Account { Username = "duongnmse170013@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170013", Fullname="Nguyen Minh Duong", Dob=new DateOnly(2001,3,19), Gender=true,  Mail="duongnmse170013@fpt.edu.vn", Phone="0909111113", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Bac Ninh", StatusId="ACTIVE"} },
        new Account { Username = "linhptse170014@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170014", Fullname="Pham Thi Linh", Dob=new DateOnly(2001,11,2), Gender=false, Mail="linhptse170014@fpt.edu.vn", Phone="0909111114", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Hai Phong",StatusId="ACTIVE"} },
        new Account { Username = "sondvse170015@fpt.edu.vn", Password="123456", RoleId=1, User=new User { RollNumber="SE170015", Fullname="Do Van Son", Dob=new DateOnly(2001,1,27), Gender=true,  Mail="sondvse170015@fpt.edu.vn", Phone="0909111115", MajorId=1, CampusId=1, CapstoneProject="FPT Tracking System", Address="Ha Noi",   StatusId="ACTIVE"} },
        new Account { Username = "minhndmentor@fpt.edu.vn", Password="123456", RoleId=2, User=new User { RollNumber="ME02", Fullname="Nguyen Duc Minh", Dob=new DateOnly(1994,3,12), Gender=true, Mail="minhndmentor@fpt.edu.vn", Phone="0912000002", MajorId=1, CampusId=1, Address="Ha Noi", StatusId="ACTIVE"} },

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

    };

        public static List<Group> GetGroupsForSemester(int semesterId)
        {
            if (semesterId == 1)
            {
                var students = Accounts.Where(a => a.RoleId == 1 && a.User.RollNumber.StartsWith("SE140")).ToList();
                var mentor = Accounts.FirstOrDefault(a => a.RoleId == 2); // dùng FirstOrDefault để tránh lỗi

                if (mentor == null)
                    throw new Exception("Không tìm thấy mentor cho semester 1");

                return new List<Group>
        {
            new Group
            {
                Code="G01",
                Name="Capstone Team Vanguard",
                SemesterId=semesterId,
                CreateAt=DateTime.Now.AddMonths(-2),
                Profession="AI Development",
                MajorId=1,
                Description="Team phát triển hệ thống AI",
                VietnameseTitle="Nhóm Tiên Phong",
                StatusId="ACTIVE",
                MeetingId=1,
                ExpireDate=DateTime.Now.AddMonths(6),
                GroupUsers=students.Select((s,i)=>new GroupUser
                {
                    User=s.User,
                    Role=i==0?"Leader":"Student",
                    IsActive=true,
                    CreateAt=DateTime.Now.AddMonths(-2),
                    UpdateAt=DateTime.Now,
                    Status="Active"
                })
                .Concat(new[]
                {
                    new GroupUser
                    {
                        User=mentor.User,
                        Role="Supervisor",
                        IsActive=true,
                        CreateAt=DateTime.Now.AddMonths(-1),
                        UpdateAt=DateTime.Now,
                        Status="Active"
                    }
                }).ToList()
            }
        };
            }

            if (semesterId == 2)
            {
                var allStudents = Accounts
                    .Where(a => a.RoleId == 1 && a.User.RollNumber.StartsWith("SE170"))
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
            "G11",
            "FPT Tracking System",
            "Hệ thống theo dõi FPT",
            "Software Engineering",
            GetStudents(1,5),
            "ME01"
        ),

        CreateGroup(
            "G12",
            "Smart Course Booking",
            "Hệ thống đăng ký môn học thông minh",
            "Software Engineering",
            GetStudents(6,10),
            "ME01"
        ),

        CreateGroup(
            "G13",
            "AI Student Progress Monitor",
            "Theo dõi tiến độ sinh viên bằng AI",
            "AI Development",
            GetStudents(11,15),
            "ME02"
        ),

        CreateGroup(
            "G14",
            "Campus Event Management System",
            "Quản lý sự kiện trong campus",
            "Software Engineering",
            GetStudents(16,20),
            "ME03"
        ),

        CreateGroup(
            "G15",
            "Internship Matching Platform",
            "Nền tảng kết nối thực tập",
            "Software Engineering",
            GetStudents(21,25),
            "ME04"
        ), CreateGroup(
        "G16",
        "Smart Attendance System",
        "Hệ thống điểm danh thông minh",
        "Software Engineering",
        GetStudents(26,30),
        "ME05"
    ),
    CreateGroup(
        "G17",
        "AI Career Recommendation",
        "Gợi ý nghề nghiệp bằng AI",
        "AI Development",
        GetStudents(31,35),
        "ME06"
    )
    };
     }


            return new List<Group>();
        }

    }
}
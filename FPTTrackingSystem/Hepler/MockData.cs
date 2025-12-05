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

        // Semester 1 mentor
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

            /*    if (semesterId == 4)
                {
                    var students = Accounts.Where(a => a.RoleId == 1 && a.User.RollNumber.StartsWith("SE170")).ToList();
                    var mentor = Accounts.FirstOrDefault(a => a.RoleId == 2 && a.User.RollNumber == "ME01");

                    if (mentor == null)
                        throw new Exception("Không tìm thấy mentor cho semester 2");

                    return new List<Group>
            {
                new Group
                {
                    Code="G11",
                    Name="FPT Tracking System",
                    SemesterId=semesterId,
                    CreateAt=DateTime.Now.AddMonths(-2),
                    Profession="Software Engineer",
                    MajorId=1,
                    Description="System Tracking Capstone Group",
                    VietnameseTitle="He thong theo doi do an",
                    StatusId="ACTIVE",
                    MeetingId=null,
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
                }*/
            if (semesterId == 2)
            {
                var allStudents = Accounts.Where(a => a.RoleId == 1 && a.User.RollNumber.StartsWith("SE170")).ToList();

                var group1Students = allStudents.Where(a =>
                    int.Parse(a.User.RollNumber.Substring(6)) >= 1 &&
                    int.Parse(a.User.RollNumber.Substring(6)) <= 5
                ).ToList();

                var group2Students = allStudents.Where(a =>
                    int.Parse(a.User.RollNumber.Substring(6)) >= 6 &&
                    int.Parse(a.User.RollNumber.Substring(6)) <= 10
                ).ToList();

                var mentor = Accounts.FirstOrDefault(a => a.RoleId == 2 && a.User.RollNumber == "ME01");

                if (mentor == null)
                    throw new Exception("Không tìm thấy mentor cho semester 2");

                return new List<Group>
    {
        // GROUP 1
        new Group
        {
            Code="G11",
            Name="FPT Tracking System",
            SemesterId=semesterId,
            CreateAt=DateTime.Now.AddMonths(-2),
            Profession="Software Engineer",
            MajorId=1,
            Description="Group 1 Capstone Team",
            VietnameseTitle="Nhóm theo dõi số 1",
            StatusId="ACTIVE",
            ExpireDate=DateTime.Now.AddMonths(6),
            GroupUsers= group1Students.Select((s,i)=>new GroupUser
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
        },

        // GROUP 2
        new Group
        {
            Code="G12",
            Name="Booking Course System",
            SemesterId=semesterId,
            CreateAt=DateTime.Now.AddMonths(-2),
            Profession="Software Engineer",
            MajorId=1,
            Description="Book Course For Student",
            VietnameseTitle="Booking Course System",
            StatusId="ACTIVE",
            ExpireDate=DateTime.Now.AddMonths(6),
            GroupUsers= group2Students.Select((s,i)=>new GroupUser
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

            return new List<Group>();
        }

    }
}
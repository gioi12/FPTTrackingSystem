using Entities.Models;

namespace FPTTrackingSystem.Helper
{
    public static class MockData
    {
        public static readonly Semester SemesterSummer2025 = new Semester
        {
            Name = "Summer 2025",
            IsActive = true,
            Description = "Summer semester for Capstone projects, from May to August 2025",
            StartAt = new DateTime(2025, 4, 28),
            EndAt = new DateTime(2025, 8, 17)
        };

        public static readonly Semester SemesterFall2025 = new Semester
        {
            Name = "Fall 2025",
            IsActive = false,
            Description = "Fall semester for Capstone projects, from September to December 2025",
            StartAt = new DateTime(2026, 8, 18),
            EndAt = new DateTime(2026, 12, 7)
        };

        public static readonly List<Semester> AllSemesters = new List<Semester>
        {   
            SemesterSummer2025,
            SemesterFall2025
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

        /*        public static readonly List<Account> Accounts = new()
        {
            // --- Students (RollNumber SE14xxxx, đổi Fullname) ---
            new Account
            {
                Username = "gioidmhe171512",
                Password = "123456",
                RoleId = 1,
                User =
                    new User
                    {
                        RollNumber = "SE140001",
                        Fullname = "Nguyễn Văn An",
                        Dob = new DateOnly(2000, 2, 14),
                        Gender = true,
                        Mail = "gioidmhe171512@fpt.edu.vn",
                        Phone = "0909123456",
                        MajorId = 1,
                        CampusId = 1,
                        CapstoneProject = "FPT Tracking System",
                        Address = "Hà Nội",
                        StatusId = "ACTIVE"
                    }
            },
            new Account
            {
                Username = "haildhe172452",
                Password = "123456",
                RoleId = 1,
                User =
                    new User
                    {
                        RollNumber = "SE140002",
                        Fullname = "Lê Thị Bích",
                        Dob = new DateOnly(2001, 6, 10),
                        Gender = true,
                        Mail = "huongtthe172436@fpt.edu.vn",
                        Phone = "0909888777",
                        MajorId = 1,
                        CampusId = 1,
                        CapstoneProject = "FPT Tracking System",
                        Address = "Đà Nẵng",
                        StatusId = "ACTIVE"
                    }
            },
            new Account
            {
                Username = "cuonghvhe176362",
                Password = "123456",
                RoleId = 1,
                User =
                    new User
                    {
                        RollNumber = "SE140003",
                        Fullname = "Trần Hoàng Cường",
                        Dob = new DateOnly(2001, 9, 21),
                        Gender = true,
                        Mail = "haildhe172452@fpt.edu.vn",
                        Phone = "0911222333",
                        MajorId = 1,
                        CampusId = 1,
                        CapstoneProject = "FPT Tracking System",
                        Address = "Hồ Chí Minh",
                        StatusId = "ACTIVE"
                    }
            },
            new Account
            {
                Username = "handghe170064",
                Password = "123456",
                RoleId = 1,
                User =
                    new User
                    {
                        RollNumber = "SE140004",
                        Fullname = "Phạm Gia Hân",
                        Dob = new DateOnly(2001, 4, 5),
                        Gender = true,
                        Mail = "handghe170064@fpt.edu.vn",
                        Phone = "0988777666",
                        MajorId = 1,
                        CampusId = 1,
                        CapstoneProject = "FPT Tracking System",
                        Address = "Hà Nội",
                        StatusId = "ACTIVE"
                    }
            },
            new Account
            {
                Username = "huongtt170064",
                Password = "123456",
                RoleId = 1,
                User =
                    new User
                    {
                        RollNumber = "SE140005",
                        Fullname = "Trịnh Thiên Hương",
                        Dob = new DateOnly(2001, 4, 5),
                        Gender = false,
                        Mail = "cuonghvhe176362@fpt.edu.vn",
                        Phone = "0988777666",
                        MajorId = 1,
                        CampusId = 1,
                        CapstoneProject = "FPT Tracking System",
                        Address = "Hà Nội",
                        StatusId = "ACTIVE"
                    }
            },
            new Account
            {
                Username = "namnthe172123",
                Password = "123456",
                RoleId = 1,
                User =
                    new User
                    {
                        RollNumber = "SE140006",
                        Fullname = "Nguyễn Thanh Nam",
                        Dob = new DateOnly(2001, 7, 12),
                        Gender = true,
                        Mail = "namnthe172123@fpt.edu.vn",
                        Phone = "0909111222",
                        MajorId = 1,
                        CampusId = 1,
                        CapstoneProject = "IoT Device Management System",
                        Address = "Hà Nội",
                        StatusId = "ACTIVE"
                    }
            },
            new Account
            {
                Username = "minhpthe171234",
                Password = "123456",
                RoleId = 1,
                User =
                    new User
                    {
                        RollNumber = "SE140007",
                        Fullname = "Phạm Tiến Minh",
                        Dob = new DateOnly(2001, 10, 8),
                        Gender = true,
                        Mail = "minhpthe171234@fpt.edu.vn",
                        Phone = "0909222333",
                        MajorId = 1,
                        CampusId = 1,
                        CapstoneProject = "IoT Device Management System",
                        Address = "Hà Nội",
                        StatusId = "ACTIVE"
                    }
            },
            new Account
            {
                Username = "anhtthe173456",
                Password = "123456",
                RoleId = 1,
                User =
                    new User
                    {
                        RollNumber = "SE140008",
                        Fullname = "Trần Thị Anh",
                        Dob = new DateOnly(2002, 1, 25),
                        Gender = false,
                        Mail = "anhtthe173456@fpt.edu.vn",
                        Phone = "0909333444",
                        MajorId = 1,
                        CampusId = 1,
                        CapstoneProject = "IoT Device Management System",
                        Address = "Hà Nội",
                        StatusId = "ACTIVE"
                    }
            },
            new Account
            {
                Username = "quangnmhe175678",
                Password = "123456",
                RoleId = 1,
                User =
                    new User
                    {
                        RollNumber = "SE140009",
                        Fullname = "Nguyễn Minh Quang",
                        Dob = new DateOnly(2002, 3, 5),
                        Gender = true,
                        Mail = "quangnmhe175678@fpt.edu.vn",
                        Phone = "0909444555",
                        MajorId = 1,
                        CampusId = 1,
                        CapstoneProject = "IoT Device Management System",
                        Address = "Hà Nội",
                        StatusId = "ACTIVE"
                    }
            },
            new Account
            {
                Username = "linhnthe176789",
                Password = "123456",
                RoleId = 1,
                User =
                    new User
                    {
                        RollNumber = "SE140010",
                        Fullname = "Nguyễn Thị Linh",
                        Dob = new DateOnly(2002, 5, 18),
                        Gender = false,
                        Mail = "linhnthe176789@fpt.edu.vn",
                        Phone = "0909555666",
                        MajorId = 1,
                        CampusId = 1,
                        CapstoneProject = "IoT Device Management System",
                        Address = "Hà Nội",
                        StatusId = "ACTIVE"
                    }
            },

            // --- Mentors giữ nguyên ---
            new Account
            {
                Username = "lampt2@gmail.com",
                Password = "123456",
                RoleId = 2,
                User =
                    new User
                    {
                        RollNumber = "ME01",
                        Fullname = "Mentor Phan Truong Lam",
                        Dob = new DateOnly(2001, 4, 5),
                        Gender = true,
                        MajorId = 1,
                        CampusId = 1,
                        Mail = "lampt2@gmail.com",
                        Phone = "0988777555",
                        Address = "Ha Noi",
                        StatusId = "ACTIVE"
                    }
            },
            new Account
            {
                Username = "sonnt5@gmail.com",
                Password = "123456",
                RoleId = 2,
                User =
                    new User
                    {
                        RollNumber = "ME02",
                        Fullname = "Mentor Ngo Tung Son",
                        Dob = new DateOnly(2001, 4, 5),
                        Gender = true,
                        MajorId = 1,
                        CampusId = 1,
                        Mail = "sonnt5@gmail.com",
                        Phone = "0988777444",
                        Address = "Ha Noi",
                        StatusId = "ACTIVE"
                    }
            },
            new Account
            {
                Username = "thanhbv@gmail.com",
                Password = "123456",
                RoleId = 2,
                User =
                    new User
                    {
                        RollNumber = "ME03",
                        Fullname = "Mentor Bui Van Thanh",
                        Dob = new DateOnly(1998, 8, 15),
                        Gender = true,
                        MajorId = 1,
                        CampusId = 1,
                        Mail = "thanhbv@gmail.com",
                        Phone = "0909666777",
                        Address = "Ha Noi",
                        StatusId = "ACTIVE"
                    }
            },
        };*/


        public static readonly List<Account> Accounts = new()
        {
            new Account
            {
                Username = "gioidmhe171512@fpt.edu.vn",
                Password = "123456",
                RoleId = 1,
                User =
                    new User
                    {
                        RollNumber = "SE170001",
                        Fullname = "Doan Manh Gioi",
                        Dob = new DateOnly(2000, 2, 14),
                        Gender = true,
                        Mail = "gioidmhe171512@fpt.edu.vn",
                        Phone = "0909123456",
                        MajorId = 1,
                        CampusId = 1,
                        CapstoneProject = "FPT Tracking System",
                        Address = "Ha Noi",
                        StatusId = "ACTIVE"
                    }
            },
            new Account
            {
                Username = "huongtthe172436@fpt.edu.vn",
                Password = "123456",
                RoleId = 1,
                User =
                    new User
                    {
                        RollNumber = "SE170002",
                        Fullname = "TT Huong",
                        Dob = new DateOnly(2001, 6, 10),
                        Gender = true,
                        Mail = "huongtthe172436@fpt.edu.vn",
                        Phone = "0909888777",
                        MajorId = 1,
                        CampusId = 1,
                        CapstoneProject = "FPT Tracking System",
                        Address = "Da Nang",
                        StatusId = "ACTIVE"
                    }
            },
            new Account
            {
                Username = "haildhe172452@fpt.edu.vn",
                Password = "123456",
                RoleId = 1,
                User =
                    new User
                    {
                        RollNumber = "SE170003",
                        Fullname = "Le Duy Hai",
                        Dob = new DateOnly(2001, 9, 21),
                        Gender = true,
                        Mail = "haildhe172452@fpt.edu.vn",
                        Phone = "0911222333",
                        MajorId = 1,
                        CampusId = 1,
                        CapstoneProject = "FPT Tracking System",
                        Address = "Ho Chi Minh",
                        StatusId = "ACTIVE"
                    }
            },
            new Account
            {
                Username = "handghe170064@fpt.edu.vn",
                Password = "123456",
                RoleId = 1,
                User =
                    new User
                    {
                        RollNumber = "SE170004",
                        Fullname = "Dinh Gia Han",
                        Dob = new DateOnly(2001, 4, 5),
                        Gender = true,
                        Mail = "handghe170064@fpt.edu.vn",
                        Phone = "0988777666",
                        MajorId = 1,
                        CampusId = 1,
                        CapstoneProject = "FPT Tracking System",
                        Address = "Ha Noi",
                        StatusId = "ACTIVE"
                    }
            },
            new Account
            {
                Username = "cuonghvhe176362@fpt.edu.vn",
                Password = "123456",
                RoleId = 1,
                User =
                    new User
                    {
                        RollNumber = "SE170005",
                        Fullname = "Ha Van Cuong",
                        Dob = new DateOnly(2001, 4, 5),
                        Gender = true,
                        Mail = "cuonghvhe176362@fpt.edu.vn",
                        Phone = "0988777666",
                        MajorId = 1,
                        CampusId = 1,
                        CapstoneProject = "FPT Tracking System",
                        Address = "Ha Noi",
                        StatusId = "ACTIVE"
                    }
            },
            new Account
            {
                Username = "lampt2@gmail.com",
                Password = "123456",
                RoleId = 2,
                User =
                    new User
                    {
                        RollNumber = "ME01",
                        Fullname = "Mentor Phan Truong Lam",
                        Dob = new DateOnly(2001, 4, 5),
                        Gender = true,
                        MajorId = 1,
                        CampusId = 1,
                        Mail = "lampt2@gmail.com",
                        Phone = "0988777555",
                        Address = "Ha Noi",
                        StatusId = "ACTIVE"
                    }
            },
            new Account
            {
                Username = "sonnt5@gmail.com",
                Password = "123456",
                RoleId = 2,
                User =
                    new User
                    {
                        RollNumber = "ME02",
                        Fullname = "Mentor Ngo Tung Son",
                        Dob = new DateOnly(2001, 4, 5),
                        Gender = true,
                        MajorId = 1,
                        CampusId = 1,
                        Mail = "sonnt5@gmail.com",
                        Phone = "0988777444",
                        Address = "Ha Noi",
                        StatusId = "ACTIVE"
                    }
            },           

        };

        // Lưu ý: Groups không có GroupUsers nữa vì UserId chưa tồn tại
        // Bạn cần tạo Groups sau khi tạo Users xong
        //    public static List<Group> GetGroups(int semesterId,
        // int user1Id, int user2Id, int user3Id, int user4Id, int user5Id, int mentor1Id,
        // int user6Id, int user7Id, int user8Id, int user9Id, int user10Id, int mentor2Id)
        //    {
        //        var groups = new List<Group>
        //{
        //    // --- 2 nhóm chính ---
        //    new Group
        //    {
        //        Code = "P01",
        //        Name = "Capstone Team Alpha",
        //        SemesterId = semesterId,
        //        CreateAt = DateTime.Now.AddMonths(-2),
        //        Profession = "AI Development",
        //        MajorId = 1,
        //        Description = "Team làm chatbot AI",
        //        VietnameseTitle = "Nhóm Alpha",
        //        StatusId = "ACTIVE",
        //        MeetingId = null,
        //        ExpireDate = SemesterSummer2025.EndAt,
        //        GroupUsers = new List<GroupUser>
        //        {
        //            new GroupUser { UserId = user1Id, Role = "Leader", IsActive = true, CreateAt = DateTime.Now.AddMonths(-2), UpdateAt = DateTime.Now, Status = "Active" },
        //            new GroupUser { UserId = user2Id, Role = "Student", IsActive = true, CreateAt = DateTime.Now.AddMonths(-2), UpdateAt = DateTime.Now, Status = "Active" },
        //            new GroupUser { UserId = user3Id, Role = "Student", IsActive = true, CreateAt = DateTime.Now.AddMonths(-2), UpdateAt = DateTime.Now, Status = "Active" },
        //            new GroupUser { UserId = user4Id, Role = "Student", IsActive = true, CreateAt = DateTime.Now.AddMonths(-2), UpdateAt = DateTime.Now, Status = "Active" },
        //            new GroupUser { UserId = user5Id, Role = "Student", IsActive = true, CreateAt = DateTime.Now.AddMonths(-2), UpdateAt = DateTime.Now, Status = "Active" },
        //            new GroupUser { UserId = mentor1Id, Role = "Supervisor", IsActive = true, CreateAt = DateTime.Now.AddMonths(-1), UpdateAt = DateTime.Now, Status = "Active" }
        //        }
        //    },
        //    new Group
        //    {
        //        Code = "P02",
        //        Name = "Capstone Team Beta",
        //        SemesterId = semesterId,
        //        CreateAt = DateTime.Now.AddMonths(-1),
        //        Profession = "IoT Development",
        //        MajorId = 1,
        //        Description = "Team building IoT device management system",
        //        VietnameseTitle = "Nhóm Beta",
        //        StatusId = "ACTIVE",
        //        MeetingId = null,
        //        ExpireDate = SemesterSummer2025.EndAt,
        //        GroupUsers = new List<GroupUser>
        //        {
        //            new GroupUser { UserId = user6Id, Role = "Leader", IsActive = true, CreateAt = DateTime.Now.AddMonths(-1), UpdateAt = DateTime.Now, Status = "Active" },
        //            new GroupUser { UserId = user7Id, Role = "Student", IsActive = true, CreateAt = DateTime.Now.AddMonths(-1), UpdateAt = DateTime.Now, Status = "Active" },
        //            new GroupUser { UserId = user8Id, Role = "Student", IsActive = true, CreateAt = DateTime.Now.AddMonths(-1), UpdateAt = DateTime.Now, Status = "Active" },
        //            new GroupUser { UserId = user9Id, Role = "Student", IsActive = true, CreateAt = DateTime.Now.AddMonths(-1), UpdateAt = DateTime.Now, Status = "Active" },
        //            new GroupUser { UserId = user10Id, Role = "Student", IsActive = true, CreateAt = DateTime.Now.AddMonths(-1), UpdateAt = DateTime.Now, Status = "Active" },
        //            new GroupUser { UserId = mentor2Id, Role = "Supervisor", IsActive = true, CreateAt = DateTime.Now, UpdateAt = DateTime.Now, Status = "Active" }
        //        }
        //    }
        //};

        //        // --- 48 group auto tạo cùng user/account ---
        //        int startUserId = 11; // giả lập id bắt đầu
        //        for (int i = 3; i <= 50; i++)
        //        {
        //            // Tạo account và user student
        //            var studentAcc = new Account
        //            {
        //                Username = $"auto_stu_{i}",
        //                Password = "123456",
        //                RoleId = 3
        //            };
        //            var studentUser = new User
        //            {
        //                Fullname = $"Auto Student {i}",
        //                RollNumber = $"AUTO_STU_{i}",
        //                Mail = $"student{i}@fpt.edu.vn",
        //                Account = studentAcc
        //            };

        //            // Tạo account và user supervisor
        //            var supervisorAcc = new Account
        //            {
        //                Username = $"auto_sup_{i}",
        //                Password = "123456",
        //                RoleId = 2
        //            };
        //            var supervisorUser = new User
        //            {
        //                Fullname = $"Auto Supervisor {i}",
        //                RollNumber = $"AUTO_SUP_{i}",
        //                Mail = $"supervisor{i}@fpt.edu.vn",
        //                Account = supervisorAcc
        //            };

        //            // Group auto
        //            var group = new Group
        //            {
        //                Code = $"P{i:D2}",
        //                Name = $"Auto Group {i}",
        //                SemesterId = semesterId,
        //                CreateAt = DateTime.Now,
        //                Description = $"Auto generated group {i}",
        //                VietnameseTitle = $"Nhóm Auto {i}",
        //                StatusId = "ACTIVE",
        //                ExpireDate = SemesterSummer2025.EndAt,
        //                GroupUsers = new List<GroupUser>
        //        {
        //            new GroupUser { User = studentUser, Role = "Student", IsActive = true, CreateAt = DateTime.Now, UpdateAt = DateTime.Now, Status = "Active" },
        //            new GroupUser { User = supervisorUser, Role = "Supervisor", IsActive = true, CreateAt = DateTime.Now, UpdateAt = DateTime.Now, Status = "Active" }
        //        }
        //            };

        //            groups.Add(group);
        //        }

        //        return groups;
        //    }


        public static List<Group> GetGroups(int semesterId,
 int user1Id, int user2Id, int user3Id, int user4Id, int user5Id, int mentor1Id,
 int user6Id, int user7Id, int user8Id, int user9Id, int user10Id, int mentor2Id)
        {
            var groups = new List<Group>
{
    // --- 2 nhóm chính đã đổi tên ---
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
        MeetingId = null,
        ExpireDate = SemesterSummer2025.EndAt,
        GroupUsers = new List<GroupUser>
        {
            new GroupUser { UserId = user1Id, Role = "Leader", IsActive = true, CreateAt = DateTime.Now.AddMonths(-2), UpdateAt = DateTime.Now, Status = "Active" },
            new GroupUser { UserId = user2Id, Role = "Student", IsActive = true, CreateAt = DateTime.Now.AddMonths(-2), UpdateAt = DateTime.Now, Status = "Active" },
            new GroupUser { UserId = user3Id, Role = "Student", IsActive = true, CreateAt = DateTime.Now.AddMonths(-2), UpdateAt = DateTime.Now, Status = "Active" },
            new GroupUser { UserId = user4Id, Role = "Student", IsActive = true, CreateAt = DateTime.Now.AddMonths(-2), UpdateAt = DateTime.Now, Status = "Active" },
            new GroupUser { UserId = user5Id, Role = "Student", IsActive = true, CreateAt = DateTime.Now.AddMonths(-2), UpdateAt = DateTime.Now, Status = "Active" },
            new GroupUser { UserId = mentor1Id, Role = "Supervisor", IsActive = true, CreateAt = DateTime.Now.AddMonths(-1), UpdateAt = DateTime.Now, Status = "Active" }
        }
    } };
//    new Group
//    {
//        Code = "G02",
//        Name = "Capstone Team Horizon",
//        SemesterId = semesterId,
//        CreateAt = DateTime.Now.AddMonths(-1),
//        Profession = "IoT Development",
//        MajorId = 1,
//        Description = "Team phát triển hệ thống IoT",
//        VietnameseTitle = "Nhóm Chân Trời",
//        StatusId = "ACTIVE",
//        MeetingId = null,
//        ExpireDate = SemesterSummer2025.EndAt,
//        GroupUsers = new List<GroupUser>
//        {
//            new GroupUser { UserId = user6Id, Role = "Leader", IsActive = true, CreateAt = DateTime.Now.AddMonths(-1), UpdateAt = DateTime.Now, Status = "Active" },
//            new GroupUser { UserId = user7Id, Role = "Student", IsActive = true, CreateAt = DateTime.Now.AddMonths(-1), UpdateAt = DateTime.Now, Status = "Active" },
//            new GroupUser { UserId = user8Id, Role = "Student", IsActive = true, CreateAt = DateTime.Now.AddMonths(-1), UpdateAt = DateTime.Now, Status = "Active" },
//            new GroupUser { UserId = user9Id, Role = "Student", IsActive = true, CreateAt = DateTime.Now.AddMonths(-1), UpdateAt = DateTime.Now, Status = "Active" },
//            new GroupUser { UserId = user10Id, Role = "Student", IsActive = true, CreateAt = DateTime.Now.AddMonths(-1), UpdateAt = DateTime.Now, Status = "Active" },
//            new GroupUser { UserId = mentor2Id, Role = "Supervisor", IsActive = true, CreateAt = DateTime.Now, UpdateAt = DateTime.Now, Status = "Active" }
//        }
//    }
//};

                //            // --- 48 group auto tạo cùng user/account ---
                //            int startUserId = 11; // giả lập id bắt đầu
                //            for (int i = 3; i <= 10; i++)
                //            {
                //                // Tạo account và user student
                //                var studentAcc = new Account
                //                {
                //                    Username = $"auto_stu_{i}",
                //                    Password = "123456",
                //                    RoleId = 3
                //                };
                //                var studentUser = new User
                //                {
                //                    Fullname = $"Auto Student {i}",
                //                    RollNumber = $"AUTO_STU_{i}",
                //                    Mail = $"student{i}@fpt.edu.vn",
                //                    Account = studentAcc
                //                };

                //                // Tạo account và user supervisor
                //                var supervisorAcc = new Account
                //                {
                //                    Username = $"auto_sup_{i}",
                //                    Password = "123456",
                //                    RoleId = 2
                //                };
                //                var supervisorUser = new User
                //                {
                //                    Fullname = $"Auto Supervisor {i}",
                //                    RollNumber = $"AUTO_SUP_{i}",
                //                    Mail = $"supervisor{i}@fpt.edu.vn",
                //                    Account = supervisorAcc
                //                };

                //                // Group auto
                //                var group = new Group
                //                {
                //                    Code = $"G{i:D2}",
                //                    Name = $"Auto Group {i}",
                //                    SemesterId = semesterId,
                //                    CreateAt = DateTime.Now,
                //                    Description = $"Auto generated group {i}",
                //                    VietnameseTitle = $"Nhóm Auto {i}",
                //                    StatusId = "ACTIVE",
                //                    ExpireDate = SemesterSummer2025.EndAt,
                //                    GroupUsers = new List<GroupUser>
                //            {
                //                new GroupUser { User = studentUser, Role = "Student", IsActive = true, CreateAt = DateTime.Now, UpdateAt = DateTime.Now, Status = "Active" },
                //                new GroupUser { User = supervisorUser, Role = "Supervisor", IsActive = true, CreateAt = DateTime.Now, UpdateAt = DateTime.Now, Status = "Active" }
                //            }
                //                };

                //                groups.Add(group);
                //            }

            return groups;
        }

    }
}
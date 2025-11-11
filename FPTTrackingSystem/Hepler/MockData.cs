using Entities.Models;

namespace FPTTrackingSystem.Helper
{
    public static class MockData
    {
        public static readonly Semester SemesterSpring2026 = new Semester
        {
            Name = "Spring 2026",
            IsActive = true,
            Description = "Spring semester for Capstone projects, from Jan to April 2026",
            StartAt = new DateTime(2026, 1, 1),
            EndAt = new DateTime(2026, 4, 30)
        };

        public static readonly Semester SemesterSummer2026 = new Semester
        {
            Name = "Summer 2026",
            IsActive = false,
            Description = "Summer semester for Capstone projects, from May to August 2026",
            StartAt = new DateTime(2026, 5, 1),
            EndAt = new DateTime(2026, 8, 31)
        };

        public static readonly Semester SemesterFall2026 = new Semester
        {
            Name = "Fall 2026",
            IsActive = false,
            Description = "Fall semester for Capstone projects, from September to December 2026",
            StartAt = new DateTime(2026, 9, 1),
            EndAt = new DateTime(2026, 12, 31)
        };

        public static readonly List<Semester> AllSemesters = new List<Semester>
        {
            SemesterSpring2026,
            SemesterSummer2026,
            SemesterFall2026
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
            new Account
            {
                Username = "gioidmhe171512@fpt.edu.vn",
                Password = "123456",
                RoleId = 1,
                Users = new List<User>
                {
                    new User
                    {
                        RollNumber = "SE150001",
                        Fullname = "Doan Manh Gioi",
                        Dob = new DateOnly(2000, 2, 14),
                        Gender = true,
                        Mail = "gioidmhe171512@fpt.edu.vn",
                        Phone = "0909123456",
                        MajorId = 1,
                        CapstoneProject = "FPT Tracking System",
                        Address = "Ha Noi",
                        StatusId = "ACTIVE"
                    }
                }
            },
            new Account
            {
                Username = "haildhe172452@fpt.edu.vn",
                Password = "123456",
                RoleId = 1,
                Users = new List<User>
                {
                    new User
                    {
                        RollNumber = "SE150002",
                        Fullname = "Le Duy Hai",
                        Dob = new DateOnly(2001, 6, 10),
                        Gender = true,
                        Mail = "haildhe172452@fpt.edu.vn",
                        Phone = "0909888777",
                        MajorId = 1,
                        CapstoneProject = "FPT Tracking System",
                        Address = "Da Nang",
                        StatusId = "ACTIVE"
                    }
                }
            },
            new Account
            {
                Username = "cuonghvhe176362@fpt.edu.vn",
                Password = "123456",
                RoleId = 1,
                Users = new List<User>
                {
                    new User
                    {
                        RollNumber = "SE150003",
                        Fullname = "Ha Van Cuong",
                        Dob = new DateOnly(2001, 9, 21),
                        Gender = true,
                        Mail = "cuonghvhe176362@fpt.edu.vn",
                        Phone = "0911222333",
                        MajorId = 1,
                        CapstoneProject = "FPT Tracking System",
                        Address = "Ho Chi Minh",
                        StatusId = "ACTIVE"
                    }
                }
            },
            new Account
            {
                Username = "handghe170064@fpt.edu.vn",
                Password = "123456",
                RoleId = 1,
                Users = new List<User>
                {
                    new User
                    {
                        RollNumber = "SE150004",
                        Fullname = "Dinh Gia Han",
                        Dob = new DateOnly(2001, 4, 5),
                        Gender = true,
                        Mail = "handghe170064@fpt.edu.vn",
                        Phone = "0988777666",
                        MajorId = 1,
                        CapstoneProject = "FPT Tracking System",
                        Address = "Ha Noi",
                        StatusId = "ACTIVE"
                    }
                }
            },
            new Account
            {
                Username = "huongtt170064@fpt.edu.vn",
                Password = "123456",
                RoleId = 1,
                Users = new List<User>
                {
                    new User
                    {
                        RollNumber = "SE150005",
                        Fullname = "Trinh Thien Huong",
                        Dob = new DateOnly(2001, 4, 5),
                        Gender = true,
                        Mail = "huongtt170064@fpt.edu.vn",
                        Phone = "0988777666",
                        MajorId = 1,
                        CapstoneProject = "FPT Tracking System",
                        Address = "Ha Noi",
                        StatusId = "ACTIVE"
                    }
                }
            },
            new Account
            {
                Username = "lampt2@gmail.com",
                Password = "123456",
                RoleId = 2,
                Users = new List<User>
                {
                    new User
                    {
                        RollNumber = "ME01",
                        Fullname = "Mentor Phan Truong Lam",
                        Dob = new DateOnly(2001, 4, 5),
                        Gender = true,
                        Mail = "lampt2@gmail.com",
                        Phone = "0988777555",
                        Address = "Ha Noi",
                        StatusId = "ACTIVE"
                    }
                }
            },
            new Account
            {
                Username = "sonnt5@gmail.com",
                Password = "123456",
                RoleId = 2,
                Users = new List<User>
                {
                    new User
                    {
                        RollNumber = "ME02",
                        Fullname = "Mentor Ngo Tung Son",
                        Dob = new DateOnly(2001, 4, 5),
                        Gender = true,
                        Mail = "sonnt5@gmail.com",
                        Phone = "0988777444",
                        Address = "Ha Noi",
                        StatusId = "ACTIVE"
                    }
                }
            }
        };

        // Lưu ý: Groups không có GroupUsers nữa vì UserId chưa tồn tại
        // Bạn cần tạo Groups sau khi tạo Users xong
        public static List<Group> GetGroups(int semesterId, int user1Id, int user2Id, int user3Id, int user4Id,int user5Id, int mentor1Id)
        {
            return new List<Group>
            {
                new Group
                {
                    Code = "G01",
                    Name = "Capstone Team Alpha",
                    SemesterId = semesterId,
                    CreateAt = DateTime.Now.AddMonths(-2),
                    Profession = "AI Development",
                    MajorId = 1,
                    Description = "Team làm chatbot AI",
                    VietnameseTitle = "Nhóm Alpha",
                    StatusId = "ACTIVE",
                    MeetingId = null,
                    GroupUsers = new List<GroupUser>
                    {
                        new GroupUser
                        {
                            UserId = user1Id,
                            Role = "Leader",
                            IsActive = true,
                            CreateAt = DateTime.Now.AddMonths(-2),
                            UpdateAt = DateTime.Now,
                            Status = "Active"
                        },
                        new GroupUser
                        {
                            UserId = user2Id,
                            Role = "Student",
                            IsActive = true,
                            CreateAt = DateTime.Now.AddMonths(-2),
                            UpdateAt = DateTime.Now,
                            Status = "Active"
                        },
                         new GroupUser
                        {
                            UserId = user3Id,
                            Role = "Student",
                            IsActive = true,
                            CreateAt = DateTime.Now.AddMonths(-2),
                            UpdateAt = DateTime.Now,
                            Status = "Active"
                        },
                         new GroupUser
                        {
                            UserId = user4Id,
                            Role = "Student",
                            IsActive = true,
                            CreateAt = DateTime.Now.AddMonths(-2),
                            UpdateAt = DateTime.Now,
                            Status = "Active"
                        },
                           new GroupUser
                        {
                            UserId = user5Id,
                            Role = "Student",
                            IsActive = true,
                            CreateAt = DateTime.Now.AddMonths(-2),
                            UpdateAt = DateTime.Now,
                            Status = "Active"
                        },
                        new GroupUser
                        {
                            UserId = mentor1Id,
                            Role = "Supervisor",
                            IsActive = true,
                            CreateAt = DateTime.Now.AddMonths(-1),
                            UpdateAt = DateTime.Now,
                            Status = "Active"
                        }
                    }
                }
            };
        }
    }
}
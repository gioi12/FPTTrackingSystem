using Entities.Models;

namespace FPTTrackingSystem.Helper
{
    public static class MockData
    {
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
                        CapstoneProject = "Capstone Team Alpha",
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
                        CapstoneProject = "Capstone Team Alpha",
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
                        MajorId = 2,
                        CapstoneProject = "Chiến dịch quảng cáo xanh",
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
                        MajorId = 2,
                        CapstoneProject = "Chiến dịch quảng cáo xanh",
                        Address = "Ha Noi",
                        StatusId = "ACTIVE"
                    }
                }
            },
            new Account
            {
                Username = "doangioi0403@gmail.com",
                Password = "123456",
                RoleId = 2,
                Users = new List<User>
                {
                    new User
                    {
                        RollNumber = "ME01",
                        Fullname = "Mentor Gioi",
                        Dob = new DateOnly(2001, 4, 5),
                        Gender = true,
                        Mail = "doangioi0403@gmail.com",
                        Phone = "0988777555",
                        Address = "Ha Noi",
                        StatusId = "ACTIVE"
                    }
                }
            },
            new Account
            {
                Username = "huongtthe172436@fpt.edu.vn",
                Password = "123456",
                RoleId = 2,
                Users = new List<User>
                {
                    new User
                    {
                        RollNumber = "ME02",
                        Fullname = "Mentor Huong",
                        Dob = new DateOnly(2001, 4, 5),
                        Gender = true,
                        Mail = "huongtthe172436@fpt.edu.vn",
                        Phone = "0988777444",
                        Address = "Ha Noi",
                        StatusId = "ACTIVE"
                    }
                }
            }
        };

        // Lưu ý: Groups không có GroupUsers nữa vì UserId chưa tồn tại
        // Bạn cần tạo Groups sau khi tạo Users xong
        public static List<Group> GetGroups(int semesterId, int user1Id, int user2Id, int user3Id, int user4Id, int mentor1Id, int mentor2Id)
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
                            UserId = mentor1Id,
                            Role = "Supervisor",
                            IsActive = true,
                            CreateAt = DateTime.Now.AddMonths(-1),
                            UpdateAt = DateTime.Now,
                            Status = "Active"
                        }
                    }
                },
                new Group
                {
                    Code = "G02",
                    Name = "Chiến dịch quảng cáo xanh",
                    SemesterId = semesterId,
                    CreateAt = DateTime.Now.AddMonths(-1),
                    Profession = "Marketing",
                    MajorId = 2,
                    Description = "Team xây dựng plan marketing",
                    VietnameseTitle = "Nhóm Marketing",
                    StatusId = "ACTIVE",
                    MeetingId = null,
                    GroupUsers = new List<GroupUser>
                    {
                        new GroupUser
                        {
                            UserId = user3Id,
                            Role = "Leader",
                            IsActive = true,
                            CreateAt = DateTime.Now.AddMonths(-1),
                            UpdateAt = DateTime.Now,
                            Status = "Active"
                        },
                        new GroupUser
                        {
                            UserId = user4Id,
                            Role = "Student",
                            IsActive = true,
                            CreateAt = DateTime.Now.AddMonths(-1),
                            UpdateAt = DateTime.Now,
                            Status = "Active"
                        },
                        new GroupUser
                        {
                            UserId = mentor2Id,
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
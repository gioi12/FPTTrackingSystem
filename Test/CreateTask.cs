using DataTranferObjects.Login;
using DataTranferObjects.Staff.Task;
using Entities.Models;
using FluentAssertions;
using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Services.Student.Implements;
using FPTTrackingSystem.Services.Token;
using FPTTrackingSystem.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Moq;
using Repositories.Common.Interfaces;
using Repositories.Staff.Interfaces;
using Repositories.Student.Interfaces;

namespace FPTTrackingSystem.Test.Services.Student
{
    [TestClass]
    public class CreateTask
    {
        private Mock<ITaskRepository> _mockTaskRepository;
        private Mock<AuthUtils> _mockAuthUtils;
        private FpttrackingSystemContext _context; // ✅ REAL CONTEXT
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private Mock<IAttachmentRepository> _mockAttachmentRepository;
        private Mock<IGroupRepository> _mockGroupRepository;
        private Mock<IGroupService> _mockGroupService;
        private Mock<IJwtService> _mockJwtService;
        private TaskService _taskService;

        [TestInitialize]
        public void Setup()
        {
            _mockTaskRepository = new Mock<ITaskRepository>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockAttachmentRepository = new Mock<IAttachmentRepository>();
            _mockGroupRepository = new Mock<IGroupRepository>();
            _mockGroupService = new Mock<IGroupService>();
            _mockJwtService = new Mock<IJwtService>();

            _mockAuthUtils = new Mock<AuthUtils>(
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<FPTTrackingSystem.Services.Login.IAccountService>(),
                _mockJwtService.Object
            );

            // 🔥 DbContext THẬT – InMemory
            var options = new DbContextOptionsBuilder<FpttrackingSystemContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new FpttrackingSystemContext(options);

            _taskService = new TaskService(
                _mockTaskRepository.Object,
                _mockJwtService.Object,
                _mockGroupService.Object,
                _mockAuthUtils.Object,
                _context, // ✅ REAL CONTEXT
                _mockHttpContextAccessor.Object,
                _mockAttachmentRepository.Object,
                _mockWebHostEnvironment.Object,
                _mockGroupRepository.Object
            );
        }

        // ================== TEST 1 ==================
        [TestMethod]
        public async System.Threading.Tasks.Task CreateTaskAsync_WithValidData_ShouldCreateSuccessfully()
        {
            var userId = 1;
            var groupId = 1;
            var endAt = DateTime.Now.AddDays(7);

            var dto = new CreateTaskDTO
            {
                GroupId = groupId,
                Name = "Test Task",
                Description = "Test Description",
                TaskType = "feature",
                Status = "todo",
                Priority = "high",
                EndAt = endAt,
                AssignedUserId = 2,
                ReviewerId = 3
            };

            _mockAuthUtils.Setup(x => x.GetUserInfoFromCookie())
                .ReturnsAsync(new UserInfo
                {
                    Id = userId,
                    Role = "Student",
                    Groups = new List<int> { groupId }
                });

            _mockJwtService.Setup(x => x.GetSemesterFromToken("test-token"))
                .Returns(new SemesterInfo { End_Time = DateTime.Now.AddMonths(4).ToString() });

            SetupHttpContext();

            _mockTaskRepository.Setup(x => x.CreateTaskAsync(
                    It.IsAny<Entities.Models.Task>(),
                    2, userId, 3))
                .ReturnsAsync(new Entities.Models.Task
                {
                    Id = 1,
                    GroupId = groupId,
                    Name = "Test Task",
                    Description = "Test Description",
                    Status = "Todo",
                    Priority = "High",
                    Type = "Feature"
                });

            var result = await _taskService.CreateTaskAsync(dto);

            result.Should().NotBeNull();
            result.Name.Should().Be("Test Task");
        }

        // ================== TEST 2 ==================
        [TestMethod]
        public async System.Threading.Tasks.Task CreateTaskAsync_StudentNotInGroup_ShouldThrowUnauthorized()
        {
            var dto = new CreateTaskDTO
            {
                GroupId = 1,
                Name = "Test",
                TaskType = "feature",
                Status = "todo",
                Priority = "high",
                EndAt = DateTime.Now.AddDays(1),
                AssignedUserId = 2
            };

            _mockAuthUtils.Setup(x => x.GetUserInfoFromCookie())
                .ReturnsAsync(new UserInfo
                {
                    Id = 1,
                    Role = "Student",
                    Groups = new List<int>() 
                });

            _mockJwtService.Setup(x => x.GetSemesterFromToken("test-token"))
                .Returns(new SemesterInfo { End_Time = DateTime.Now.AddMonths(4).ToString() });

            SetupHttpContext();

            Func<System.Threading.Tasks.Task> act = async () => await _taskService.CreateTaskAsync(dto);

            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        // ================== TEST 3 ==================
        [TestMethod]
        public async System.Threading.Tasks.Task CreateTaskAsync_InvalidStatus_ShouldThrowArgumentException()
        {
            var dto = new CreateTaskDTO
            {
                GroupId = 1,
                Name = "Test",
                TaskType = "feature",
                Status = "invalid",
                Priority = "high",
                EndAt = DateTime.Now.AddDays(1),
                AssignedUserId = 2
            };

            _mockAuthUtils.Setup(x => x.GetUserInfoFromCookie())
                .ReturnsAsync(new UserInfo
                {
                    Id = 1,
                    Role = "Student",
                    Groups = new List<int> { 1 }
                });

            _mockJwtService.Setup(x => x.GetSemesterFromToken("test-token"))
                .Returns(new SemesterInfo { End_Time = DateTime.Now.AddMonths(4).ToString() });

            SetupHttpContext();

            Func<System.Threading.Tasks.Task> act = async () => await _taskService.CreateTaskAsync(dto);

            await act.Should().ThrowAsync<ArgumentException>();
        }

        private void SetupHttpContext()
        {
            var ctx = new Mock<HttpContext>();
            var req = new Mock<HttpRequest>();
            var cookies = new Mock<IRequestCookieCollection>();
            cookies.Setup(c => c["token"]).Returns("test-token");
            req.Setup(r => r.Cookies).Returns(cookies.Object);
            ctx.Setup(c => c.Request).Returns(req.Object);
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(ctx.Object);
        }
    }
}

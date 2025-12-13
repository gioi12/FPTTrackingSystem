using DataTranferObjects.Login;
using DataTranferObjects.Student.Meeting;
using Entities.Models;
using FluentAssertions;
using FPTTrackingSystem.Services.Student.Implements;
using FPTTrackingSystem.Services.Student.Interfaces;
using FPTTrackingSystem.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Repositories.Staff.Interfaces;
using Repositories.Student.Interfaces;

namespace FPTTrackingSystem.Test.Services.Student
{
    [TestClass]
    public class FinalizeScheduleTests
    {
        private Mock<IMeetingRepository> _mockMeetingRepository;
        private Mock<IGroupRepository> _mockGroupRepository;
        private Mock<AuthUtils> _mockAuthUtils;
        private FpttrackingSystemContext _context;
        private MeetingService _meetingService;

        [TestInitialize]
        public void Setup()
        {
            // Repo mocks
            _mockMeetingRepository = new Mock<IMeetingRepository>();
            _mockGroupRepository = new Mock<IGroupRepository>();

            // AuthUtils mock (QUAN TRỌNG)
            _mockAuthUtils = new Mock<AuthUtils>(
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<FPTTrackingSystem.Services.Login.IAccountService>(),
                Mock.Of<FPTTrackingSystem.Services.Token.IJwtService>()
            );

            // DbContext thật – InMemory (KHÔNG MOCK)
            var options = new DbContextOptionsBuilder<FpttrackingSystemContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new FpttrackingSystemContext(options);

            // Service
            _meetingService = new MeetingService(
                _mockMeetingRepository.Object,
                _mockAuthUtils.Object,
                _context,
                _mockGroupRepository.Object
            );
        }

        [TestMethod]
        public async System.Threading.Tasks.Task FinalizeScheduleAsync_WithValidData_ShouldReturnFinalizedSchedule()
        {
            // Arrange
            var userId = 1;
            var groupId = 1;
            var slotId = 1;
            var userInfo = new UserInfo{Id = userId,Role = "Supervisor"};
            var slot = new Slot{ Id = slotId,NameSlot = "Slot 1",StartAt = new TimeOnly(9, 0),EndAt = new TimeOnly(11, 0)};

            _context.Slots.Add(slot);
            await _context.SaveChangesAsync();

            var meeting = new Meeting
            {
                Id = 10,
                DayOfWeek = "Monday",
                MeetingLink = "https://meet.google.com/abc",
                SlotId = slotId,
                Slot = slot,
                IsActive = true,
                CreateAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow
            };

            var request = new FinalizeScheduleRequestDto
            {
                FinalMeeting = new FinalMeetingDto
                {
                    Day = "Monday",
                    SlotId = slotId,
                    MeetingLink = "https://meet.google.com/abc"
                }
            };

            _mockAuthUtils.Setup(x => x.GetUserInfoFromCookie()).ReturnsAsync(userInfo);
            _mockMeetingRepository.Setup(x => x.CheckStudentInGroupAsync(userId, groupId)).ReturnsAsync(true);
            _mockMeetingRepository.Setup(x => x.FinalizeOrUpdateScheduleAsync(groupId, It.IsAny<FinalMeetingDto>(), userId)).ReturnsAsync(meeting);

            var result = await _meetingService.FinalizeScheduleAsync(groupId, request);

            result.Should().NotBeNull();
            result.FinalMeeting.Should().NotBeNull();
            result.FinalMeeting.Id.Should().Be(10);
            result.FinalMeeting.Day.Should().Be("Monday");
            result.FinalMeeting.MeetingLink.Should().Be("https://meet.google.com/abc");
            result.FinalMeeting.Slot.Should().NotBeNull();
            result.FinalMeeting.Slot.Id.Should().Be(slotId);
            result.FinalMeeting.Slot.NameSlot.Should().Be("Slot 1");
        }

        [TestMethod]
        public async System.Threading.Tasks.Task FinalizeScheduleAsync_NotSupervisor_ShouldThrowException()
        {
            // Arrange
            var userInfo = new UserInfo
            {
                Id = 1,
                Role = "Student"
            };

            _mockAuthUtils
                .Setup(x => x.GetUserInfoFromCookie())
                .ReturnsAsync(userInfo);

            var request = new FinalizeScheduleRequestDto
            {
                FinalMeeting = new FinalMeetingDto()
            };

            Func<System.Threading.Tasks.Task> act = async () => await _meetingService.FinalizeScheduleAsync(1, request);

            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [TestMethod]
        public async System.Threading.Tasks.Task FinalizeScheduleAsync_NotMentorOfGroup_ShouldThrowException()
        {
            // Arrange
            var userId = 1;
            var groupId = 1;

            var userInfo = new UserInfo
            {
                Id = userId,
                Role = "Supervisor"
            };

            _mockAuthUtils
                .Setup(x => x.GetUserInfoFromCookie())
                .ReturnsAsync(userInfo);

            _mockMeetingRepository
                .Setup(x => x.CheckStudentInGroupAsync(userId, groupId))
                .ReturnsAsync(false);

            var request = new FinalizeScheduleRequestDto
            {
                FinalMeeting = new FinalMeetingDto()
            };

            // Act & Assert
            Func<System.Threading.Tasks.Task> act = async () => await _meetingService.FinalizeScheduleAsync(1, request);

            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }
    }
}

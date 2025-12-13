using DataTranferObjects.Login;
using DataTranferObjects.Student.Meeting;
using Entities.Models;
using FluentAssertions;
using FPTTrackingSystem.Services.Student.Implements;
using FPTTrackingSystem.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Repositories.Staff.Interfaces;
using Microsoft.EntityFrameworkCore;

using Repositories.Student.Interfaces;

namespace FPTTrackingSystem.Test.Services.Student
{
    [TestClass]
    public class CreateOrUpdateFreeTimeSlots
    {
        private Mock<IMeetingRepository> _mockMeetingRepository;
        private Mock<IGroupRepository> _groupRepo;
        private Mock<AuthUtils> _mockAuthUtils;
        private FpttrackingSystemContext _context;
        private MeetingService _meetingService;

        [TestInitialize]
        public void Setup()
        {
            _mockMeetingRepository = new Mock<IMeetingRepository>();
            _groupRepo = new Mock<IGroupRepository>();

            _mockAuthUtils = new Mock<AuthUtils>(
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<FPTTrackingSystem.Services.Login.IAccountService>(),
                Mock.Of<FPTTrackingSystem.Services.Token.IJwtService>()
            );

            var options = new DbContextOptionsBuilder<FpttrackingSystemContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new FpttrackingSystemContext(options);

            _meetingService = new MeetingService(
                _mockMeetingRepository.Object,
                _mockAuthUtils.Object,
                _context,                 
                _groupRepo.Object
            );
        }


        [TestMethod]
        public async System.Threading.Tasks.Task CreateOrUpdateFreeTimeSlotsAsync_WithNewSlots_ShouldCreateSuccessfully()
        {
            var userId = 1;
            var groupId = 1;

            var userInfo = new UserInfo
            {
                Id = userId,
                Name = "Test Student",
                Role = "Student"
            };

            var requests = new List<FreeTimeSlotRequest>
    {
        new FreeTimeSlotRequest
        {
            DayOfWeek = "Monday",
            Slots = new List<int> { 1, 2 }
        },
        new FreeTimeSlotRequest
        {
            DayOfWeek = "Wednesday",
            Slots = new List<int> { 3 }
        }
    };

            _mockAuthUtils
                .Setup(x => x.GetUserInfoFromCookie())
                .ReturnsAsync(userInfo);

            _mockMeetingRepository
                .Setup(x => x.GetUserSlotsAsync(userId, groupId))
                .ReturnsAsync(new List<UserSlot>());

            _mockMeetingRepository
                .Setup(x => x.AddUserSlotsAsync(It.IsAny<List<UserSlot>>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            // Act
            await _meetingService.CreateOrUpdateFreeTimeSlotsAsync(groupId, requests);

            // Assert
            _mockMeetingRepository.Verify(x =>
                x.AddUserSlotsAsync(It.Is<List<UserSlot>>(slots =>
                    slots.Count == 3 &&
                    slots.Count(s => s.DayOfWeek == "Monday") == 2 &&
                    slots.Count(s => s.DayOfWeek == "Wednesday") == 1 &&
                    slots.All(s => s.UserId == userId && s.GroupId == groupId)
                )),
                Times.Once);
        }


        [TestMethod]
        public async System.Threading.Tasks.Task CreateOrUpdateFreeTimeSlotsAsync_WithExistingSlots_ShouldDeleteAndRecreate()
        {
            var userId = 1;
            var groupId = 1;

            var userInfo = new UserInfo
            {
                Id = userId,
                Role = "Student"
            };

            var existingSlots = new List<UserSlot>
    {
        new UserSlot
        {
            UserId = userId,
            GroupId = groupId,
            SlotId = 99,
            DayOfWeek = "Monday"
        }
    };

            var requests = new List<FreeTimeSlotRequest>
    {
        new FreeTimeSlotRequest
        {
            DayOfWeek = "Monday",
            Slots = new List<int> { 1 }
        }
    };

            _mockAuthUtils
                .Setup(x => x.GetUserInfoFromCookie())
                .ReturnsAsync(userInfo);

            _mockMeetingRepository
                .Setup(x => x.GetUserSlotsAsync(userId, groupId))
                .ReturnsAsync(existingSlots);

            _mockMeetingRepository
                .Setup(x => x.DeleteUserSlotsAsync(It.IsAny<List<UserSlot>>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            _mockMeetingRepository
                .Setup(x => x.AddUserSlotsAsync(It.IsAny<List<UserSlot>>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            // Act
            await _meetingService.CreateOrUpdateFreeTimeSlotsAsync(groupId, requests);

            // Assert
            _mockMeetingRepository.Verify(
                x => x.DeleteUserSlotsAsync(It.Is<List<UserSlot>>(l => l.Count == 1)),
                Times.Once);

            _mockMeetingRepository.Verify(
                x => x.AddUserSlotsAsync(It.Is<List<UserSlot>>(l =>
                    l.Count == 1 && l[0].SlotId == 1)),
                Times.Once);
        }

    }
}


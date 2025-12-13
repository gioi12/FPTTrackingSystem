using DataTranferObjects.Login;
using DataTranferObjects.Student.Meeting;
using Entities.Models;
using FluentAssertions;
using FPTTrackingSystem.Services.Student.Implements;
using FPTTrackingSystem.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Repositories.Staff.Interfaces;
using Repositories.Student.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FPTTrackingSystem.Test.Services.Student
{
    [TestClass]
    public class UpdateMeetingMinus
    {
        private Mock<IMeetingRepository> _mockMeetingRepository;
        private Mock<AuthUtils> _mockAuthUtils;
        private Mock<IGroupRepository> _groupRepo;
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

            // ✅ InMemory DbContext (để service không bị crash)
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

        // ==========================================================
        // TEST 1: UPDATE THÀNH CÔNG
        // ==========================================================
        [TestMethod]
        public async System.Threading.Tasks.Task UpdateMeetingMinute_WithValidData_ShouldUpdateSuccessfully()
        {
            // Arrange
            var minuteId = 1;
            var userId = 1;

            var startAt = DateTime.Now;
            var endAt = DateTime.Now.AddHours(2);

            var request = new MeetingMinuteUpdateReq
            {
                Id = minuteId,
                startAt = startAt,
                endAt = endAt,
                Attendance = "All members present",
                Issue = "No issues",
                MeetingContent = "Discussed progress",
                Other = "Next meeting soon"
            };

            var userInfo = new UserInfo { Id = userId };

            var existingMinute = new MeetingMinute
            {
                Id = minuteId,
                StartAt = DateTime.Now.AddHours(-1),
                EndAt = DateTime.Now,
                Attendance = "Old",
                Issue = "Old",
                MeetingContent = "Old",
                Other = "Old"
            };

            _mockAuthUtils
                .Setup(x => x.GetUserInfoFromCookie())
                .ReturnsAsync(userInfo);

            _mockMeetingRepository
                .Setup(x => x.GetMeetingMinuteById(minuteId))
                .ReturnsAsync(existingMinute);

            _mockMeetingRepository
                .Setup(x => x.UpdateMeetingMinute(It.IsAny<MeetingMinute>()))
                .ReturnsAsync((MeetingMinute m) => m);

            // Act
            var result = await _meetingService.UpdateMeetingMinute(request);

            // Assert – DTO RESPONSE (CHỈ ASSERT FIELD SERVICE TRẢ VỀ)
            result.Should().NotBeNull();
            result.Id.Should().Be(minuteId);
            result.Attendance.Should().Be(request.Attendance);
            result.Issue.Should().Be(request.Issue);
            result.MeetingContent.Should().Be(request.MeetingContent);
            result.Other.Should().Be(request.Other);

            // ❌ KHÔNG ASSERT startAt / endAt vì service KHÔNG MAP

            // Assert – ENTITY UPDATE (QUAN TRỌNG NHẤT)
            _mockMeetingRepository.Verify(
                x => x.UpdateMeetingMinute(It.Is<MeetingMinute>(m =>
                    m.Id == minuteId &&
                    m.StartAt == startAt &&
                    m.EndAt == endAt &&
                    m.Attendance == request.Attendance &&
                    m.Issue == request.Issue &&
                    m.MeetingContent == request.MeetingContent &&
                    m.Other == request.Other
                )),
                Times.Once
            );
        }

        // ==========================================================
        // TEST 2: NOT FOUND → THROW EXCEPTION
        // ==========================================================
        [TestMethod]
        public async System.Threading.Tasks.Task UpdateMeetingMinute_NotFound_ShouldThrowException()
        {
            // Arrange
            var minuteId = 1;

            var request = new MeetingMinuteUpdateReq
            {
                Id = minuteId,
                startAt = DateTime.Now,
                endAt = DateTime.Now.AddHours(1),
                Attendance = "Test",
                MeetingContent = "Test"
            };

            _mockAuthUtils
                .Setup(x => x.GetUserInfoFromCookie())
                .ReturnsAsync(new UserInfo { Id = 1 });

            _mockMeetingRepository
                .Setup(x => x.GetMeetingMinuteById(minuteId))
                .ReturnsAsync((MeetingMinute?)null);

            // Act
            Func<System.Threading.Tasks.Task> act = async () =>
                await _meetingService.UpdateMeetingMinute(request);

            // Assert
            await act.Should()
                .ThrowAsync<ValidationException>()
                .WithMessage("*Not found*");
        }
    }
}

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
    public class UpdateMeetingScheduleDate
    {
        private Mock<IMeetingRepository> _mockMeetingRepository;
        private Mock<AuthUtils> _mockAuthUtils;
        private FpttrackingSystemContext _context;
        private MeetingService _meetingService;
        private Mock<IGroupRepository> _groupRepo;
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

            // ✅ InMemory DbContext (BẮT BUỘC)
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

        // =========================================================
        // TEST 1: UPDATE THÀNH CÔNG
        // =========================================================
      
        [TestMethod]
        public async System.Threading.Tasks.Task UpdateMeetingScheduleDateAsync_WithValidData_ShouldUpdateSuccessfully()
        {
            // Arrange
            var scheduleId = 1;
            var meetingDate = DateTime.Today.AddDays(1); // ✅ FUTURE DATE
            var newStartAt = "09:00";
            var newEndAt = "11:00";
            var newDescription = "Updated meeting";

            var existingSchedule = new MeetingScheduleDate
            {
                Id = scheduleId,
                MeetingDate = meetingDate,
                StartAt = new TimeOnly(8, 0),
                EndAt = new TimeOnly(10, 0),
                Description = "Original meeting",
                IsActive = true,
                Meeting = new Meeting()
            };

            var dto = new UpdateMeetingScheduleDateDto
            {
                MeetingDate = meetingDate,
                StartAt = newStartAt,
                EndAt = newEndAt,
                Description = newDescription,
                IsActive = true
            };

            _mockMeetingRepository
                .Setup(x => x.GetByIdAsync(scheduleId))
                .ReturnsAsync(existingSchedule);

            _mockMeetingRepository
                .Setup(x => x.UpdateAsync(It.IsAny<MeetingScheduleDate>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            // Act
            var result = await _meetingService
                .UpdateMeetingScheduleDateAsync(scheduleId, dto);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(scheduleId);
            result.Data.MeetingDate.Should().Be(meetingDate);
            result.Data.StartAt.Should().Be(TimeOnly.Parse(newStartAt));
            result.Data.EndAt.Should().Be(TimeOnly.Parse(newEndAt));
            result.Data.Description.Should().Be(newDescription);

            _mockMeetingRepository.Verify(
                x => x.UpdateAsync(It.IsAny<MeetingScheduleDate>()),
                Times.Once);
        }

        // =========================================================
        // TEST 2: NOT FOUND
        // =========================================================
        [TestMethod]
        public async System.Threading.Tasks.Task UpdateMeetingScheduleDateAsync_ScheduleNotFound_ShouldThrowException()
        {
            _mockMeetingRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((MeetingScheduleDate?)null);

            Func<System.Threading.Tasks.Task> act = async () =>
                await _meetingService.UpdateMeetingScheduleDateAsync(
                    1,
                    new UpdateMeetingScheduleDateDto()
                );

            await act.Should()
                .ThrowAsync<Exception>() // ✅ ĐÚNG TYPE
                .WithMessage("*does not exist*"); // hoặc "*deleted*"
        }

        // =========================================================
        // TEST 3: MEETING ĐÃ DIỄN RA
        // =========================================================
        [TestMethod]
        public async System.Threading.Tasks.Task UpdateMeetingScheduleDateAsync_MeetingAlreadyOccurred_ShouldThrowException()
        {
            var existing = new MeetingScheduleDate
            {
                Id = 1,
                MeetingDate = DateTime.Today.AddDays(-1), // đã xảy ra
                Meeting = new Meeting()
            };

            _mockMeetingRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(existing);

            Func<System.Threading.Tasks.Task> act = async () =>
                await _meetingService.UpdateMeetingScheduleDateAsync(
                    1,
                    new UpdateMeetingScheduleDateDto
                    {
                        MeetingDate = DateTime.Today.AddDays(1)
                    }
                );

            await act.Should()
                .ThrowAsync<Exception>() // ✅ ĐÚNG TYPE
                .WithMessage("*already occurred*"); // ✅ match message
        }

        // =========================================================
        // TEST 4: TIME FORMAT SAI
        // =========================================================
        [TestMethod]
        public async System.Threading.Tasks.Task UpdateMeetingScheduleDateAsync_InvalidTimeFormat_ShouldThrowException()
        {
            var existing = new MeetingScheduleDate
            {
                Id = 1,
                MeetingDate = DateTime.Today.AddDays(2),
                Meeting = new Meeting()
            };

            _mockMeetingRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(existing);

            Func<System.Threading.Tasks.Task> act = async () =>
                await _meetingService.UpdateMeetingScheduleDateAsync(
                    1,
                    new UpdateMeetingScheduleDateDto { StartAt = "abc" }
                );

            await act.Should()
                .ThrowAsync<Exception>()                // ✅ ĐÚNG TYPE
                .WithMessage("*Invalid StartAt format*"); // ✅ ĐÚNG MESSAGE
        }


        // =========================================================
        // TEST 5: START > END
        // =========================================================
        [TestMethod]
        public async System.Threading.Tasks.Task UpdateMeetingScheduleDateAsync_StartTimeAfterEndTime_ShouldThrowException()
        {
            var existing = new MeetingScheduleDate
            {
                Id = 1,
                MeetingDate = DateTime.Today.AddDays(2),
                Meeting = new Meeting()
            };

            _mockMeetingRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(existing);

            Func<System.Threading.Tasks.Task> act = async () =>
                await _meetingService.UpdateMeetingScheduleDateAsync(
                    1,
                    new UpdateMeetingScheduleDateDto
                    {
                        StartAt = "11:00",
                        EndAt = "09:00"
                    }
                );

            await act.Should()
                .ThrowAsync<Exception>() // ✅ ĐÚNG TYPE
                .WithMessage("*Start time must be earlier than end time*");
        }
    }
}

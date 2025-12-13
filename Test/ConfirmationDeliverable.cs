using DataTranferObjects.Enum;
using DataTranferObjects.Login;
using Entities.Models;
using FluentAssertions;
using FPTTrackingSystem.Services.Staff.Implementations;
using FPTTrackingSystem.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;

using Moq;
using Repositories.Common.Interfaces;
using Repositories.Staff.Interfaces;

namespace FPTTrackingSystem.Test.Services.Staff
{
    [TestClass]
    public class ConfirmDeliverable
    {
        private Mock<IDeliverableRepository> _mockDeliverableRepository;
        private Mock<ISemesterRepository> _mockSemesterRepository;
        private Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private Mock<IAttachmentRepository> _mockAttachmentRepository;
        private Mock<IGroupRepository> _mockGroupRepository;
        private Mock<AuthUtils> _mockAuthUtils;
        private DeliverableService _deliverableService;

        [TestInitialize]
        public void Setup()
        {
            _mockDeliverableRepository = new Mock<IDeliverableRepository>();
            _mockSemesterRepository = new Mock<ISemesterRepository>();
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockAttachmentRepository = new Mock<IAttachmentRepository>();
            _mockGroupRepository = new Mock<IGroupRepository>();
            _mockAuthUtils = new Mock<AuthUtils>(
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<FPTTrackingSystem.Services.Login.IAccountService>(),
                Mock.Of<FPTTrackingSystem.Services.Token.IJwtService>()
            );

            _deliverableService = new DeliverableService(
                _mockDeliverableRepository.Object,
                _mockSemesterRepository.Object,
                _mockWebHostEnvironment.Object,
                _mockAttachmentRepository.Object,
                _mockAuthUtils.Object,
                _mockGroupRepository.Object
            );
        }

        [TestMethod]
        public async System.Threading.Tasks.Task ConfirmDeliverable_BeforeDeadline_ShouldReturnConfirmed()
        {
            // Arrange
            var userId = 1;
            var groupId = 1;
            var deliverableId = 1;
            var note = "Good work";
            var semesterId = 1;

            var userInfo = new UserInfo { Id = userId, Name = "Test User" };
            var group = new Group
            {
                Id = groupId,
                Name = "Test Group",
                SemesterId = semesterId,
                GroupUsers = new List<GroupUser>
                {
                    new GroupUser { UserId = userId }
                }
            };

            var semester = new Semester
            {
                Id = semesterId,
                StartAt = DateTime.Now.AddDays(-30),
                EndAt = DateTime.Now.AddDays(60),
                SemesterVacations = new List<SemesterVacation>()
            };

            var deliverableGroup = new DeliverableGroup
            {
                Id = 1,
                GroupId = groupId,
                DeliverableId = deliverableId,
                Status = ProgressEnum.Pending
            };

            var deliverable = new Deliverable
            {
                Id = deliverableId,
                Name = "Deliverable 1",
                Deadline = "Week 10 - Friday - 23:59",// Week 5
                DeliverableGroups = new List<DeliverableGroup> { deliverableGroup }
            };

            _mockAuthUtils.Setup(x => x.GetUserInfoFromCookie()).ReturnsAsync(userInfo);
            _mockGroupRepository.Setup(x => x.GetByIdAsync(groupId)).ReturnsAsync(group);
            _mockDeliverableRepository.Setup(x => x.GetById(deliverableId)).ReturnsAsync(deliverable);
            _mockSemesterRepository.Setup(x => x.GetSemesterByIdAsync(semesterId)).ReturnsAsync(semester);
            _mockDeliverableRepository.Setup(x => x.UpdateDeliverable(It.IsAny<Deliverable>())).Returns(System.Threading.Tasks.Task.CompletedTask);

            // Act
            var result = await _deliverableService.ConfirmDeliverable(groupId, deliverableId, note);

            // Assert
            result.Should().Be(ProgressEnum.Confirmed);

            _mockDeliverableRepository.Verify(x => x.UpdateDeliverable(
                It.Is<Deliverable>(d =>
                    d.DeliverableGroups.Any(dg =>
                        dg.GroupId == groupId &&
                        dg.DeliverableId == deliverableId &&
                        dg.Status == ProgressEnum.Confirmed &&
                        dg.Note == note))),
                Times.Once);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task ConfirmDeliverable_AfterDeadline_ShouldReturnLate()
        {
            // Arrange
            var userId = 1;
            var groupId = 1;
            var deliverableId = 1;
            var note = "Late submission";
            var semesterId = 1;

            var userInfo = new UserInfo { Id = userId, Name = "Test User" };
            var group = new Group
            {
                Id = groupId,
                Name = "Test Group",
                SemesterId = semesterId,
                GroupUsers = new List<GroupUser>
                {
                    new GroupUser { UserId = userId }
                }
            };

            var semester = new Semester
            {
                Id = semesterId,
                StartAt = DateTime.Now.AddDays(-60),
                EndAt = DateTime.Now.AddDays(30),
                SemesterVacations = new List<SemesterVacation>()
            };

            var deliverableGroup = new DeliverableGroup
            {
                Id = 1,
                GroupId = groupId,
                DeliverableId = deliverableId,
                Status = ProgressEnum.Pending
            };

            var deliverable = new Deliverable
            {
                Id = deliverableId,
                Name = "Deliverable 1",
                Deadline = "Week 10 - Friday - 23:59", // Week 1 - đã qua
                DeliverableGroups = new List<DeliverableGroup> { deliverableGroup }
            };

            _mockAuthUtils.Setup(x => x.GetUserInfoFromCookie()).ReturnsAsync(userInfo);
            _mockGroupRepository.Setup(x => x.GetByIdAsync(groupId)).ReturnsAsync(group);
            _mockDeliverableRepository.Setup(x => x.GetById(deliverableId)).ReturnsAsync(deliverable);
            _mockSemesterRepository.Setup(x => x.GetSemesterByIdAsync(semesterId)).ReturnsAsync(semester);
            _mockDeliverableRepository.Setup(x => x.UpdateDeliverable(It.IsAny<Deliverable>())).Returns(System.Threading.Tasks.Task.CompletedTask);

            // Act
            var result = await _deliverableService.ConfirmDeliverable(groupId, deliverableId, note);

            // Assert
            result.Should().Be(ProgressEnum.Confirmed);

            _mockDeliverableRepository.Verify(x => x.UpdateDeliverable(
     It.Is<Deliverable>(d =>
         d.DeliverableGroups.Any(dg =>
             dg.Status == ProgressEnum.Confirmed &&
             dg.Note == note))),
     Times.Once);

        }

        [TestMethod]
        public async System.Threading.Tasks.Task ConfirmDeliverable_UnsubmittedStatus_ShouldThrowException()
        {
            // Arrange
            var userId = 1;
            var groupId = 1;
            var deliverableId = 1;
            var semesterId = 1;

            var userInfo = new UserInfo { Id = userId, Name = "Test User" };
            var group = new Group
            {
                Id = groupId,
                Name = "Test Group",
                SemesterId = semesterId,
                GroupUsers = new List<GroupUser>
                {
                    new GroupUser { UserId = userId }
                }
            };

            var semester = new Semester
            {
                Id = semesterId,
                StartAt = DateTime.Now.AddDays(-30),
                EndAt = DateTime.Now.AddDays(60),
                SemesterVacations = new List<SemesterVacation>()
            };

            var deliverableGroup = new DeliverableGroup
            {
                Id = 1,
                GroupId = groupId,
                DeliverableId = deliverableId,
                Status = ProgressEnum.Unsubmitted
            };

            var deliverable = new Deliverable
            {
                Id = deliverableId,
                Name = "Deliverable 1",
                Deadline = "W5",
                DeliverableGroups = new List<DeliverableGroup> { deliverableGroup }
            };

            _mockAuthUtils.Setup(x => x.GetUserInfoFromCookie()).ReturnsAsync(userInfo);
            _mockGroupRepository.Setup(x => x.GetByIdAsync(groupId)).ReturnsAsync(group);
            _mockDeliverableRepository.Setup(x => x.GetById(deliverableId)).ReturnsAsync(deliverable);
            _mockSemesterRepository.Setup(x => x.GetSemesterByIdAsync(semesterId)).ReturnsAsync(semester);

            Func<System.Threading.Tasks.Task> act = async () =>
    await _deliverableService.ConfirmDeliverable(groupId, deliverableId, null);

            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*Not submitted*");

        }

        [TestMethod]
        public async System.Threading.Tasks.Task ConfirmDeliverable_GroupNotFound_ShouldThrowException()
        {
            // Arrange
            var groupId = 1;
            var deliverableId = 1;
            var userInfo = new UserInfo { Id = 1, Name = "Test User" };

            _mockAuthUtils.Setup(x => x.GetUserInfoFromCookie()).ReturnsAsync(userInfo);
            _mockGroupRepository.Setup(x => x.GetByIdAsync(groupId)).ReturnsAsync((Group?)null);


            Func<System.Threading.Tasks.Task> act = async () =>
    await _deliverableService.ConfirmDeliverable(groupId, deliverableId, null);

            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*Not found group*");


            // Act & Assert
        }

        [TestMethod]
        public async System.Threading.Tasks.Task ConfirmDeliverable_NoPermission_ShouldThrowException()
        {
            // Arrange
            var userId = 1;
            var groupId = 1;
            var deliverableId = 1;

            var userInfo = new UserInfo { Id = userId, Name = "Test User" };
            var group = new Group
            {
                Id = groupId,
                Name = "Test Group",
                GroupUsers = new List<GroupUser>() // User không có trong group
            };

            _mockAuthUtils.Setup(x => x.GetUserInfoFromCookie()).ReturnsAsync(userInfo);
            _mockGroupRepository.Setup(x => x.GetByIdAsync(groupId)).ReturnsAsync(group);

            Func<System.Threading.Tasks.Task> act = async () =>
    await _deliverableService.ConfirmDeliverable(groupId, deliverableId, null);

            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*Not permission*");

        }
    }
}


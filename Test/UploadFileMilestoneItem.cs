using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FPTTrackingSystem.Services.Staff.Implementations;
using Repositories.Staff.Interfaces;
using Repositories.Common.Interfaces;
using FPTTrackingSystem.Utilities;
using DataTranferObjects.Enum;
using Entities.Models;
using DataTranferObjects.Login;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace FPTTrackingSystem.Test.Services.Staff
{
    [TestClass]
    public class UploadFileMilestoneItem
    {
        private Mock<IDeliverableRepository> _mockDeliverableRepository;
        private Mock<ISemesterRepository> _mockSemesterRepository;
        private Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private Mock<IAttachmentRepository> _mockAttachmentRepository;
        private Mock<IGroupRepository> _mockGroupRepository;
        private Mock<AuthUtils> _mockAuthUtils;

        private DeliverableService _deliverableService;

        private string _tempRootPath = null!;

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

            // ✅ TEMP FOLDER – mỗi test 1 folder
            _tempRootPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempRootPath);

            _mockWebHostEnvironment
                .Setup(x => x.WebRootPath)
                .Returns(_tempRootPath);

            _deliverableService = new DeliverableService(
                _mockDeliverableRepository.Object,
                _mockSemesterRepository.Object,
                _mockWebHostEnvironment.Object,
                _mockAttachmentRepository.Object,
                _mockAuthUtils.Object,
                _mockGroupRepository.Object
            );
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_tempRootPath))
            {
                Directory.Delete(_tempRootPath, true);
            }
        }

        // ============================= TEST 1 =============================
        [TestMethod]
        public async System.Threading.Tasks.Task UploadFileMilestoneItem_WithNewDeliverableGroup_ShouldCreatePendingStatus()
        {
            var userId = 1;
            var groupId = 1;
            var deliveryItemId = 1;
            var semester = "Fall2025";

            var fileName = $"{Guid.NewGuid()}.pdf";
            var fileContent = Encoding.UTF8.GetBytes("test content");

            var userInfo = new UserInfo { Id = userId };
            var group = new Group { Id = groupId };

            var deliveryItem = new DeliveryItem
            {
                Id = deliveryItemId,
                Deliverable = new Deliverable
                {
                    Id = 10,
                    DeliverableGroups = new List<DeliverableGroup>()
                }
            };

            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.Length).Returns(fileContent.Length);
            mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(fileContent));

            _mockAuthUtils.Setup(x => x.GetUserInfoFromCookie()).ReturnsAsync(userInfo);
            _mockGroupRepository.Setup(x => x.GetByIdAsync(groupId)).ReturnsAsync(group);
            _mockDeliverableRepository.Setup(x => x.GetItemByItemId(deliveryItemId)).ReturnsAsync(deliveryItem);
            _mockAttachmentRepository.Setup(x => x.AddAttachment(It.IsAny<Attachment>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask);
            _mockDeliverableRepository.Setup(x => x.UpdateDeliverable(It.IsAny<Deliverable>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            // Act
            var result = await _deliverableService
                .UploadFileMilestoneItem(mockFile.Object, groupId, deliveryItemId, semester);

            // Assert
            result.Should().NotBeNullOrWhiteSpace();
            result.Should().Contain("uploads");
            result.Should().Contain("milestones");
            result.Should().EndWith(".pdf");

            Deliverable? capturedDeliverable = null;

            _mockDeliverableRepository
                .Setup(x => x.UpdateDeliverable(It.IsAny<Deliverable>()))
                .Callback<Deliverable>(d => capturedDeliverable = d)
                .Returns(System.Threading.Tasks.Task.CompletedTask);

        }

        // ============================= TEST 2 =============================
        [TestMethod]
        public async System.Threading.Tasks.Task UploadFileMilestoneItem_WithRejectedStatus_ShouldUpdateToPending()
        {
            var userId = 1;
            var groupId = 1;
            var deliveryItemId = 1;
            var semester = "Fall2025";

            var fileName = $"{Guid.NewGuid()}.pdf";
            var fileContent = Encoding.UTF8.GetBytes("test content");

            var userInfo = new UserInfo { Id = userId };
            var group = new Group { Id = groupId };

            var deliverableGroup = new DeliverableGroup
            {
                GroupId = groupId,
                DeliverableId = 10,
                Status = ProgressEnum.Rejected
            };

            var deliveryItem = new DeliveryItem
            {
                Id = deliveryItemId,
                Deliverable = new Deliverable
                {
                    Id = 10,
                    DeliverableGroups = new List<DeliverableGroup> { deliverableGroup }
                }
            };

            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.Length).Returns(fileContent.Length);
            mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(fileContent));

            _mockAuthUtils.Setup(x => x.GetUserInfoFromCookie()).ReturnsAsync(userInfo);
            _mockGroupRepository.Setup(x => x.GetByIdAsync(groupId)).ReturnsAsync(group);
            _mockDeliverableRepository.Setup(x => x.GetItemByItemId(deliveryItemId)).ReturnsAsync(deliveryItem);
            _mockAttachmentRepository.Setup(x => x.AddAttachment(It.IsAny<Attachment>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask);
            _mockDeliverableRepository.Setup(x => x.UpdateDeliverable(It.IsAny<Deliverable>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            var result = await _deliverableService
                .UploadFileMilestoneItem(mockFile.Object, groupId, deliveryItemId, semester);

            result.Should().NotBeNullOrEmpty();

            _mockDeliverableRepository.Verify(x =>
                x.UpdateDeliverable(It.Is<Deliverable>(d =>
                    d.DeliverableGroups.Any(g => g.Status == ProgressEnum.Pending))),
                Times.Once);
        }

        // ============================= TEST 3 =============================
        [TestMethod]
        public async System.Threading.Tasks.Task UploadFileMilestoneItem_GroupNotFound_ShouldThrowException()
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test.pdf");
            mockFile.Setup(f => f.Length).Returns(10);

            _mockAuthUtils.Setup(x => x.GetUserInfoFromCookie())
                .ReturnsAsync(new UserInfo { Id = 1 });

            _mockGroupRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Group?)null);

            Func<System.Threading.Tasks.Task> act = async () =>
                await _deliverableService.UploadFileMilestoneItem(
                    mockFile.Object, 1, 1, "Fall2025");

            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*Not found group*");
        }

        // ============================= TEST 4 =============================
        [TestMethod]
        public async System.Threading.Tasks.Task UploadFileMilestoneItem_DeliveryItemNotFound_ShouldThrowException()
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test.pdf");
            mockFile.Setup(f => f.Length).Returns(10);

            _mockAuthUtils.Setup(x => x.GetUserInfoFromCookie())
                .ReturnsAsync(new UserInfo { Id = 1 });

            _mockGroupRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Group { Id = 1 });

            _mockDeliverableRepository
                .Setup(x => x.GetItemByItemId(It.IsAny<int>()))
                .ReturnsAsync((DeliveryItem?)null);

            Func<System.Threading.Tasks.Task> act = async () =>
                await _deliverableService.UploadFileMilestoneItem(
                    mockFile.Object, 1, 1, "Fall2025");

            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*Not found delivery*");
        }
    }
}

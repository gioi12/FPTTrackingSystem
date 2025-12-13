using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FPTTrackingSystem.Services.Staff.Implementations;
using Repositories.Staff.Interfaces;
using FPTTrackingSystem.Services.Common.Interfaces;
using FPTTrackingSystem.Utilities;
using DataTranferObjects.Staff.Request;
using DataTranferObjects.Staff.Response;
using Entities.Models;
using DataTranferObjects.Login;
using FPTTrackingSystem.Services.Login;
using FPTTrackingSystem.Services.Token;
using FluentAssertions;
using Microsoft.AspNetCore.Http;


namespace FPTTrackingSystem.Test.Services.Staff
{
    [TestClass]
    public class MilestoneServiceTests
    {
        private Mock<IMilestoneRepository> _mockMilestoneRepository;
        private Mock<IDeliverableRepository> _mockDeliverableRepository;
        private Mock<ISemesterRepository> _mockSemesterRepository;
        private Mock<ILogService> _mockLogService;
        private Mock<AuthUtils> _mockAuthUtils;
        private Mock<IGroupRepository> _mockGroupRepository;
        private MilestoneService _milestoneService;

        [TestInitialize]
        public void Setup()
        {
            _mockMilestoneRepository = new Mock<IMilestoneRepository>();
            _mockDeliverableRepository = new Mock<IDeliverableRepository>();
            _mockSemesterRepository = new Mock<ISemesterRepository>();
            _mockLogService = new Mock<ILogService>();
            _mockAuthUtils = new Mock<AuthUtils>(
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IAccountService>(),
                Mock.Of<IJwtService>()
            );
            _mockGroupRepository = new Mock<IGroupRepository>();

            _milestoneService = new MilestoneService(
                _mockGroupRepository.Object,
                _mockMilestoneRepository.Object,
                _mockAuthUtils.Object,
                _mockLogService.Object,
                _mockDeliverableRepository.Object,
                _mockSemesterRepository.Object
            );
        }

        [TestMethod]
        public async System.Threading.Tasks.Task CreateMilestoneInSemester_WithActiveSemester_ShouldCreateMilestoneAndDeliverable()
        {
            // Arrange
            var userId = 1; var majorCateId = 1;var semesterId = 1;
            var request = new List<MilestoneCreateRequest>
            {
                new MilestoneCreateRequest
                {Name = "Milestone 1",
                    Description = "Description 1",
                    MajorCateId = majorCateId
                },
                new MilestoneCreateRequest
                {
                    Name = "Milestone 2",
                    Description = "Description 2",
                    MajorCateId = majorCateId
                }
            };

            var userInfo = new UserInfo
            {
                Id = userId,
                Name = "Test User"
            };

            var activeSemester = new Semester
            {
                Id = semesterId,
                Name = "Spring 2024",
                IsActive = true,
                StartAt = DateTime.Now,
                EndAt = DateTime.Now.AddMonths(4)
            };

            var createdMilestones = new List<Milestone>
            {
                new Milestone
                {
                    Id = 1,
                    Name = "Milestone 1",
                    Description = "Description 1",
                    MajorId = majorCateId,
                    CreateAt = DateTime.Now,
                    CreateBy = userId,
                    IsActive = true,
                    Deliverables = new List<Deliverable>
                    {
                        new Deliverable
                        {
                            Id = 1,
                            Name = "Milestone 1",
                            Description = "Description 1",
                            SemesterId = semesterId,
                            IsActive = true,
                            MajorId = majorCateId
                        }
                    }
                },
                new Milestone
                {
                    Id = 2,
                    Name = "Milestone 2",
                    Description = "Description 2",
                    MajorId = majorCateId,
                    CreateAt = DateTime.Now,
                    CreateBy = userId,
                    IsActive = true,
                    Deliverables = new List<Deliverable>
                    {
                        new Deliverable
                        {
                            Id = 2,
                            Name = "Milestone 2",
                            Description = "Description 2",
                            SemesterId = semesterId,
                            IsActive = true,
                            MajorId = majorCateId
                        }
                    }
                }
            };

            _mockAuthUtils.Setup(x => x.GetUserInfoFromCookie())
                .ReturnsAsync(userInfo);
            _mockSemesterRepository.Setup(x => x.findActive())
                .ReturnsAsync(activeSemester);
            _mockMilestoneRepository.Setup(x => x.NewMilestontes(It.IsAny<List<Milestone>>(), majorCateId))
                .ReturnsAsync(createdMilestones);
            _mockLogService.Setup(x => x.AddRangeLogAsync(It.IsAny<List<Log>>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            // Act
            var result = await _milestoneService.CreateMilestoneInSemester(request);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            result.Data[0].Name.Should().Be("Milestone 1");
            result.Data[1].Name.Should().Be("Milestone 2");

            // Verify repository calls
            _mockMilestoneRepository.Verify(x => x.NewMilestontes(
                It.Is<List<Milestone>>(m => m.Count == 2 &&
                    m.All(mil => mil.Name != null &&
                                mil.MajorId == majorCateId &&
                                mil.CreateBy == userId &&
                                mil.Deliverables.Count == 1)),
                majorCateId),
                Times.Once);

            // Verify log service was called with milestone and deliverable logs
            _mockLogService.Verify(x => x.AddRangeLogAsync(
                It.Is<List<Log>>(logs => logs.Count == 4 && // 2 milestone logs + 2 deliverable logs
                    logs.Count(l => l.EntityName == "Milestone") == 2 &&
                    logs.Count(l => l.EntityName == "Deliverable") == 2)),
                Times.Once);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task CreateMilestoneInSemester_WithoutActiveSemester_ShouldCreateMilestoneOnly()
        {
            // Arrange
            var userId = 1;
            var majorCateId = 1;
            var request = new List<MilestoneCreateRequest>
            {
                new MilestoneCreateRequest
                {
                    Name = "Milestone 1",
                    Description = "Description 1",
                    MajorCateId = majorCateId
                }
            };

            var userInfo = new UserInfo
            {
                Id = userId,
                Name = "Test User"
            };

            var createdMilestones = new List<Milestone>
            {
                new Milestone
                {
                    Id = 1,
                    Name = "Milestone 1",
                    Description = "Description 1",
                    MajorId = majorCateId,
                    CreateAt = DateTime.Now,
                    CreateBy = userId,
                    IsActive = true,
                    Deliverables = new List<Deliverable>()
                }
            };

            _mockAuthUtils.Setup(x => x.GetUserInfoFromCookie())
                .ReturnsAsync(userInfo);
            _mockSemesterRepository.Setup(x => x.findActive())
                .ReturnsAsync((Semester?)null);
            _mockMilestoneRepository.Setup(x => x.NewMilestontes(It.IsAny<List<Milestone>>(), majorCateId))
                .ReturnsAsync(createdMilestones);
            _mockLogService.Setup(x => x.AddRangeLogAsync(It.IsAny<List<Log>>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            // Act
            var result = await _milestoneService.CreateMilestoneInSemester(request);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(1);
            result.Data[0].Name.Should().Be("Milestone 1");

            // Verify repository calls
            _mockMilestoneRepository.Verify(x => x.NewMilestontes(
                It.Is<List<Milestone>>(m => m.Count == 1 &&
                    m[0].Name == "Milestone 1" &&
                    m[0].Deliverables.Count == 0),
                majorCateId),
                Times.Once);

            // Verify log service was called only with milestone logs (no deliverable logs)
            _mockLogService.Verify(x => x.AddRangeLogAsync(
                It.Is<List<Log>>(logs => logs.Count == 1 &&
                    logs[0].EntityName == "Milestone")),
                Times.Once);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task CreateMilestoneInSemester_WithMultipleMilestones_ShouldCreateAllMilestones()
        {
            var majorCateId = 1; var semesterId = 1;

            var req = new List<MilestoneCreateRequest> {
        new() { Name="Milestone 1", Description="Desc 1", MajorCateId=majorCateId },
        new() { Name="Milestone 2", Description="Desc 2", MajorCateId=majorCateId },
        new() { Name="Milestone 3", Description="Desc 3", MajorCateId=majorCateId }
    };

            _mockAuthUtils.Setup(x => x.GetUserInfoFromCookie())
                .ReturnsAsync(new UserInfo { Id = 1, Name = "Test User" });
            _mockSemesterRepository.Setup(x => x.findActive())
                .ReturnsAsync(new Semester { Id = semesterId, IsActive = true });

            _mockMilestoneRepository.Setup(x => x.NewMilestontes(It.IsAny<List<Milestone>>(), majorCateId))
                .ReturnsAsync(req.Select((r, i) => new Milestone
                {
                    Id = i + 1,
                    Name = r.Name,
                    Description = r.Description,
                    MajorId = majorCateId,
                    CreateBy = 1,
                    IsActive = true
                }).ToList());

            _mockLogService.Setup(x => x.AddRangeLogAsync(It.IsAny<List<Log>>())).Returns(System.Threading.Tasks.Task.CompletedTask);

            var result = await _milestoneService.CreateMilestoneInSemester(req);

            result.Should().NotBeNull();
            result.Status.Should().Be(200);
            result.Data.Should().HaveCount(3);
            result.Data.Select(x => x.Name).Should().BeEquivalentTo(new[] { "Milestone 1", "Milestone 2", "Milestone 3" });

            _mockMilestoneRepository.Verify(x => x.NewMilestontes(It.Is<List<Milestone>>(m => m.Count == 3), majorCateId), Times.Once);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task CreateMilestoneInSemester_ShouldSetCorrectProperties()
        {
            // Arrange
            var userId = 1;
            var majorCateId = 1;
            var semesterId = 1;
            var request = new List<MilestoneCreateRequest>
            {
                new MilestoneCreateRequest
                {
                    Name = "Test Milestone",
                    Description = "Test Description",
                    MajorCateId = majorCateId
                }
            };

            var userInfo = new UserInfo { Id = userId, Name = "Test User" };
            var activeSemester = new Semester { Id = semesterId, Name = "Spring 2024", IsActive = true };

            var createdMilestone = new Milestone
            {
                Id = 1,
                Name = "Test Milestone",
                Description = "Test Description",
                MajorId = majorCateId,
                CreateAt = DateTime.Now,
                CreateBy = userId,
                IsActive = true,
                Deliverables = new List<Deliverable>
                {
                    new Deliverable
                    {
                        Id = 1,
                        Name = "Test Milestone",
                        Description = "Test Description",
                        SemesterId = semesterId,
                        IsActive = true,
                        MajorId = majorCateId
                    }
                }
            };

            _mockAuthUtils.Setup(x => x.GetUserInfoFromCookie()).ReturnsAsync(userInfo);
            _mockSemesterRepository.Setup(x => x.findActive()).ReturnsAsync(activeSemester);
            _mockMilestoneRepository.Setup(x => x.NewMilestontes(It.IsAny<List<Milestone>>(), majorCateId))
                .ReturnsAsync(new List<Milestone> { createdMilestone });
            _mockLogService.Setup(x => x.AddRangeLogAsync(It.IsAny<List<Log>>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            // Act
            var result = await _milestoneService.CreateMilestoneInSemester(request);

            // Assert
            _mockMilestoneRepository.Verify(x => x.NewMilestontes(
                It.Is<List<Milestone>>(m =>
                    m[0].Name == "Test Milestone" &&
                    m[0].Description == "Test Description" &&
                    m[0].MajorId == majorCateId &&
                    m[0].CreateBy == userId &&
                    m[0].IsActive == true &&
                    m[0].Deliverables.Count == 1 &&
                    m[0].Deliverables.First().Name == "Test Milestone" &&
                    m[0].Deliverables.First().SemesterId == semesterId &&
                    m[0].Deliverables.First().MajorId == majorCateId),
                majorCateId),
                Times.Once);
        }
    }
}


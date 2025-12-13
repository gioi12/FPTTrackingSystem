using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FPTTrackingSystem.Services.Staff.Implementations;
using Repositories.Staff.Interfaces;
using Repositories.Authentication;
using Repositories.Common.Interfaces;
using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Utilities;
using Entities.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FPTTrackingSystem.Services.Token;

namespace FPTTrackingSystem.Test.Services.Staff
{
    [TestClass]
    public class CreateMockData
    {
        private Mock<IGroupRepository> _mockGroupRepository;
        private Mock<IMajorRepository> _mockMajorRepository;
        private Mock<ISemesterService> _mockSemesterService;
        private Mock<AuthUtils> _mockAuthUtils;
        private Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private Mock<IAttachmentRepository> _mockAttachmentRepository;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<IAccountRepository> _mockAccountRepository;
        private Mock<ISemesterRepository> _mockSemesterRepository;
        private Mock<IJwtService> _mockJwtService;

        private GroupService _groupService;

        [TestInitialize]
        public void Setup()
        {
            _mockGroupRepository = new Mock<IGroupRepository>();
            _mockMajorRepository = new Mock<IMajorRepository>();
            _mockSemesterService = new Mock<ISemesterService>();
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockAttachmentRepository = new Mock<IAttachmentRepository>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockAccountRepository = new Mock<IAccountRepository>();
            _mockSemesterRepository = new Mock<ISemesterRepository>();
            _mockJwtService = new Mock<IJwtService>();

            // AuthUtils là class, method GetUserInfoFromCookie() là virtual => mock được
            _mockAuthUtils = new Mock<AuthUtils>(
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<FPTTrackingSystem.Services.Login.IAccountService>(),
                _mockJwtService.Object
            );

            // ✅ QUAN TRỌNG: truyền ĐÚNG thứ tự constructor
            _groupService = new GroupService(
                _mockGroupRepository.Object,
                _mockJwtService.Object,                 // IJwtService
                _mockMajorRepository.Object,            // IMajorRepository
                _mockSemesterService.Object,            // ISemesterService
                _mockAuthUtils.Object,                  // AuthUtils
                _mockWebHostEnvironment.Object,         // IWebHostEnvironment
                _mockAttachmentRepository.Object,       // IAttachmentRepository
                _mockHttpContextAccessor.Object,        // IHttpContextAccessor
                _mockAccountRepository.Object,          // IAccountRepository
                _mockSemesterRepository.Object           // ISemesterRepository
            );
        }

        [TestMethod]
        public async System.Threading.Tasks.Task CreateMockData_WithNewData_ShouldCreateSuccessfully()
        {
            // Arrange
            var semesterId = 1;

            var semester = new Semester{Id = semesterId,Name = "Fall 2025",StartAt = DateTime.Now.AddDays(-30),EndAt = DateTime.Now.AddDays(60)};

            _mockMajorRepository.Setup(x => x.FindByCodeAsync(It.IsAny<string>())).ReturnsAsync((MajorCategory?)null);
            _mockMajorRepository.Setup(x => x.CreateAsync(It.IsAny<MajorCategory>())).ReturnsAsync(true);
            _mockAccountRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<Account, bool>>>())).ReturnsAsync(new List<Account>());
            _mockAccountRepository.Setup(x => x.CreateUsers(It.IsAny<List<Account>>())).Returns(System.Threading.Tasks.Task.FromResult(new List<Account>()));
            _mockGroupRepository .Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<Group, bool>>>())).ReturnsAsync(new List<Group>());

            // NOTE: nếu CreateGroups là Task<bool> -> ReturnsAsync(true)
            _mockGroupRepository
    .Setup(x => x.CreateGroups(It.IsAny<List<Group>>()))
    .Returns(System.Threading.Tasks.Task.CompletedTask);
            _mockSemesterRepository
                .Setup(x => x.GetSemesterByIdAsync(semesterId))
                .ReturnsAsync(semester);

            // Act
            var result = await _groupService.CreateMockData(semesterId);

            // Assert
            result.Should().NotBeNull();

            _mockMajorRepository.Verify(x => x.CreateAsync(It.IsAny<MajorCategory>()), Times.AtLeastOnce);
            _mockAccountRepository.Verify(x => x.CreateUsers(It.IsAny<List<Account>>()), Times.AtLeastOnce);
            _mockGroupRepository.Verify(x => x.CreateGroups(It.IsAny<List<Group>>()), Times.AtLeastOnce);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task CreateMockData_WithExistingData_ShouldUpdateSuccessfully()
        {
            // Arrange
            var semesterId = 1;
            var majorCode = "SWP391";

            var semester = new Semester
            {
                Id = semesterId,
                Name = "Fall 2025",
                StartAt = DateTime.Now.AddDays(-30),
                EndAt = DateTime.Now.AddDays(60)
            };

            var existingMajor = new MajorCategory
            {
                Id = 1,
                Code = majorCode,
                Name = "Software Project",
                IsActive = false
            };

            var existingAccount = new Account
            {
                Id = 1,
                Username = "testuser",
                Password = "oldpassword",
                RoleId = 1,
                User = new User
                {
                    Id = 1,
                    RollNumber = "HE123456",
                    Fullname = "Test User",
                    Mail = "test@fpt.edu.vn"
                }
            };

            var existingGroup = new Group
            {
                Id = 1,
                Code = "GROUP001",
                SemesterId = semesterId,
                Name = "Existing Group",
                GroupUsers = new List<GroupUser>()
            };

            _mockSemesterRepository
                .Setup(x => x.GetSemesterByIdAsync(semesterId))
                .ReturnsAsync(semester);

            // Major đã tồn tại -> Update
            _mockMajorRepository
                .Setup(x => x.FindByCodeAsync(It.IsAny<string>()))
                .ReturnsAsync(existingMajor);

            _mockMajorRepository
                .Setup(x => x.UpdateAsync(It.IsAny<MajorCategory>()))
                .ReturnsAsync(true);


            // Account đã tồn tại -> Update
            _mockAccountRepository
                .Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<Account, bool>>>()))
                .ReturnsAsync(new List<Account> { existingAccount });

            _mockAccountRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Account>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            // Group đã tồn tại -> Update
            _mockGroupRepository
                .Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<Group, bool>>>()))
                .ReturnsAsync(new List<Group> { existingGroup });

            _mockGroupRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Group>()))
                .ReturnsAsync(existingGroup);

            // Act
            var result = await _groupService.CreateMockData(semesterId);

            // Assert
            result.Should().NotBeNull();

            _mockMajorRepository.Verify(x => x.UpdateAsync(It.IsAny<MajorCategory>()), Times.AtLeastOnce);
            _mockAccountRepository.Verify(
                x => x.CreateUsers(It.IsAny<List<Account>>()),
                Times.AtLeastOnce
            );
            _mockGroupRepository.Verify(
                x => x.CreateGroups(It.IsAny<List<Group>>()),
                Times.AtLeastOnce
            );
        }
    }
}

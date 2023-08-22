namespace Tests.Services
{
    using Microsoft.AspNetCore.Identity;

    using Moq;

    using NUnit.Framework;

    using Shouldly;

    using AutoMapper;

    using Domain.Entities;

    using Application.Interfaces.Services;

    using Infrastructure.Services.Todo;

    [TestFixture]
    public class TodoServiceTests
    {
        private Mock<IMapper> _mapperMock;
        private Mock<UserManager<User>> _userManagerMock;
        private Mock<ITodoBackgroundJobService> _backgroundJobServiceMock;
        private Mock<IOrderService> _orderServiceMock;
        private TodoService _todoService;

        [SetUp]
        public void SetUp()
        {
            _mapperMock = new Mock<IMapper>();
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _backgroundJobServiceMock = new Mock<ITodoBackgroundJobService>();
            _orderServiceMock = new Mock<IOrderService>();

            _todoService = new TodoService(
                _mapperMock.Object,
                _userManagerMock.Object,
                _backgroundJobServiceMock.Object,
                _orderServiceMock.Object);
        }

        [Test]
        public async Task GetUserTodoListsAsync_UserNotFound_ReturnsFailure()
        {
            // Arrange
            var userId = "non-existing-user";
            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _todoService.GetUserTodoListsAsync(userId);

            // Assert
            result.Success.ShouldBeFalse();
            result.Errors.ShouldContain("No Such User");
        }
    }
}

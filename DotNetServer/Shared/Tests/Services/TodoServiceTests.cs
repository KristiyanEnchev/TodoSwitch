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
    using Models.Todo;

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

        #region GetUserTodoListsAsync Tests

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

        [Test]
        public async Task GetUserTodoListsAsync_UserFound_ReturnsEmptyTodoLists()
        {
            // Arrange
            var userId = "existing-user";
            var user = new User
            {
                Id = userId,
                TodoLists = new List<TodoList>()
            };
            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _todoService.GetUserTodoListsAsync(userId);

            // Assert
            result.Success.ShouldBeTrue();
            result.Data.ShouldBeEmpty();
        }

        [Test]
        public async Task GetUserTodoListsAsync_UserFound_ReturnsTodoLists()
        {
            // Arrange
            var userId = "existing-user";
            var user = new User
            {
                Id = userId,
                TodoLists = new List<TodoList>
                {
                    new TodoList { Id = "list1", OrderIndex = 1 },
                    new TodoList { Id = "list2", OrderIndex = 2 }
                }
            };
            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _mapperMock.Setup(m => m.Map<GetUserTodoListsDto>(It.IsAny<TodoList>()))
                .Returns((TodoList source) => new GetUserTodoListsDto
                {
                    Id = source.Id,
                    OrderIndex = source.OrderIndex
                });

            // Act
            var result = await _todoService.GetUserTodoListsAsync(userId);

            // Assert
            result.Success.ShouldBeTrue();
            result.Data.ShouldNotBeEmpty();
            result.Data.Count.ShouldBe(2);
        }

        #endregion

        #region ToggleTodoItemDoneStatusAsync Tests

        [Test]
        public async Task ToggleTodoItemDoneStatusAsync_UserNotFound_ReturnsFailure()
        {
            // Arrange
            var userId = "non-existing-user";
            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _todoService.ToggleTodoItemDoneStatusAsync(userId, "listId", "itemId");

            // Assert
            result.Success.ShouldBeFalse();
            result.Errors.ShouldContain("No Such User");
        }

        [Test]
        public async Task ToggleTodoItemDoneStatusAsync_ListNotFound_ReturnsFailure()
        {
            // Arrange
            var userId = "existing-user";
            var user = new User
            {
                Id = userId,
                TodoLists = new List<TodoList>()
            };
            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _todoService.ToggleTodoItemDoneStatusAsync(userId, "non-existing-list", "itemId");

            // Assert
            result.Success.ShouldBeFalse();
            result.Errors.ShouldContain("No Such List for this User");
        }

        [Test]
        public async Task ToggleTodoItemDoneStatusAsync_ItemFound_TogglesStatus()
        {
            // Arrange
            var userId = "existing-user";
            var listId = "existing-list";
            var itemId = "existing-item";
            var todoItem = new TodoItem { Id = itemId, IsDone = false };
            var user = new User
            {
                Id = userId,
                TodoLists = new List<TodoList>
                {
                    new TodoList
                    {
                        Id = listId,
                        TodoItems = new List<TodoItem> { todoItem }
                    }
                }
            };
            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _mapperMock.Setup(m => m.Map<TodoItemDto>(todoItem))
                .Returns(new TodoItemDto { Id = itemId, IsDone = true });

            // Act
            var result = await _todoService.ToggleTodoItemDoneStatusAsync(userId, listId, itemId);

            // Assert
            result.Success.ShouldBeTrue();
            result.Data.IsDone.ShouldBeTrue();
            _backgroundJobServiceMock.Verify(bg => bg.QueueUpdateTodoItemAsync(itemId, It.IsAny<Action<TodoItem>>()), Times.Once);
            _userManagerMock.Verify(um => um.UpdateAsync(user), Times.Once);
        }

        #endregion

        #region CreateTodoItemAsync Tests

        [Test]
        public async Task CreateTodoItemAsync_UserNotFound_ReturnsFailure()
        {
            // Arrange
            var userId = "non-existing-user";
            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync((User)null);

            var todoItemDto = new CreateTodoItemDto
            {
                ListId = "listId",
                Title = "New Todo Item"
            };

            // Act
            var result = await _todoService.CreateTodoItemAsync(userId, todoItemDto);

            // Assert
            result.Success.ShouldBeFalse();
            result.Errors.ShouldContain("No Such User");
        }

        [Test]
        public async Task CreateTodoItemAsync_ListNotFound_ReturnsFailure()
        {
            // Arrange
            var userId = "existing-user";
            var user = new User
            {
                Id = userId,
                TodoLists = new List<TodoList>()
            };
            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync(user);

            var todoItemDto = new CreateTodoItemDto
            {
                ListId = "non-existing-list",
                Title = "New Todo Item"
            };

            // Act
            var result = await _todoService.CreateTodoItemAsync(userId, todoItemDto);

            // Assert
            result.Success.ShouldBeFalse();
            result.Errors.ShouldContain("No Such List for this User");
        }

        [Test]
        public async Task CreateTodoItemAsync_Success_ReturnsCreatedTodoItem()
        {
            // Arrange
            var userId = "existing-user";
            var listId = "existing-list";
            var user = new User
            {
                Id = userId,
                TodoLists = new List<TodoList>
                {
                    new TodoList { Id = listId, TodoItems = new List<TodoItem>() }
                }
            };
            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync(user);

            var todoItemDto = new CreateTodoItemDto
            {
                ListId = listId,
                Title = "New Todo Item"
            };

            var mappedTodoItem = new TodoItem { Id = "new-item-id", Title = todoItemDto.Title };
            _mapperMock.Setup(m => m.Map<TodoItem>(todoItemDto)).Returns(mappedTodoItem);
            _mapperMock.Setup(m => m.Map<TodoItemDto>(mappedTodoItem)).Returns(new TodoItemDto { Id = "new-item-id", Title = "New Todo Item" });

            // Act
            var result = await _todoService.CreateTodoItemAsync(userId, todoItemDto);

            // Assert
            result.Success.ShouldBeTrue();
            result.Data.Title.ShouldBe("New Todo Item");
            _userManagerMock.Verify(um => um.UpdateAsync(user), Times.Once);
            _backgroundJobServiceMock.Verify(bg => bg.QueueCreateTodoItemAsync(mappedTodoItem), Times.Once);
        }

        #endregion
    }
}

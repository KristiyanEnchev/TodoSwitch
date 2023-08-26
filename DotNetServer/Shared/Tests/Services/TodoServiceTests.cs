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

        #region GetTodoItemByIdAsync Tests

        [Test]
        public async Task GetTodoItemByIdAsync_UserNotFound_ReturnsFailure()
        {
            // Arrange
            var userId = "non-existing-user";
            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _todoService.GetTodoItemByIdAsync(userId, "listId", "itemId");

            // Assert
            result.Success.ShouldBeFalse();
            result.Errors.ShouldContain("No Such User");
        }

        [Test]
        public async Task GetTodoItemByIdAsync_ListNotFound_ReturnsFailure()
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
            var result = await _todoService.GetTodoItemByIdAsync(userId, "non-existing-list", "itemId");

            // Assert
            result.Success.ShouldBeFalse();
            result.Errors.ShouldContain("No Such List for this User");
        }

        [Test]
        public async Task GetTodoItemByIdAsync_ItemNotFound_ReturnsFailure()
        {
            // Arrange
            var userId = "existing-user";
            var listId = "existing-list";
            var user = new User
            {
                Id = userId,
                TodoLists = new List<TodoList>
                {
                    new TodoList { Id = listId }
                }
            };
            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _todoService.GetTodoItemByIdAsync(userId, listId, "non-existing-item");

            // Assert
            result.Success.ShouldBeFalse();
            result.Errors.ShouldContain("No Such Item for this List");
        }

        [Test]
        public async Task GetTodoItemByIdAsync_ItemFound_ReturnsTodoItem()
        {
            // Arrange
            var userId = "existing-user";
            var listId = "existing-list";
            var itemId = "existing-item";
            var todoItem = new TodoItem { Id = itemId, Title = "Sample Item" };
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

            _mapperMock.Setup(m => m.Map<GetUserTodoItemDto>(todoItem))
                .Returns(new GetUserTodoItemDto { Title = "Sample Item" });

            // Act
            var result = await _todoService.GetTodoItemByIdAsync(userId, listId, itemId);

            // Assert
            result.Success.ShouldBeTrue();
            result.Data.Title.ShouldBe("Sample Item");
        }

        #endregion

        #region UpdateTodoItemPriorityAsync Tests

        [Test]
        public async Task UpdateTodoItemPriorityAsync_UserNotFound_ReturnsFailure()
        {
            // Arrange
            var userId = "non-existing-user";
            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _todoService.UpdateTodoItemPriorityAsync(userId, "listId", "itemId", PriorityLevel.High);

            // Assert
            result.Success.ShouldBeFalse();
            result.Errors.ShouldContain("No Such User");
        }

        [Test]
        public async Task UpdateTodoItemPriorityAsync_ListNotFound_ReturnsFailure()
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
            var result = await _todoService.UpdateTodoItemPriorityAsync(userId, "non-existing-list", "itemId", PriorityLevel.High);

            // Assert
            result.Success.ShouldBeFalse();
            result.Errors.ShouldContain("No Such List for this User");
        }

        [Test]
        public async Task UpdateTodoItemPriorityAsync_ItemFound_ChangesPriority()
        {
            // Arrange
            var userId = "existing-user";
            var listId = "existing-list";
            var itemId = "existing-item";
            var todoItem = new TodoItem { Id = itemId, Priority = PriorityLevel.Low };
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
                .Returns(new TodoItemDto { Id = itemId, Priority = PriorityLevel.High });

            // Act
            var result = await _todoService.UpdateTodoItemPriorityAsync(userId, listId, itemId, PriorityLevel.High);

            // Assert
            result.Success.ShouldBeTrue();
            result.Data.Priority.ShouldBe(PriorityLevel.High);
            _backgroundJobServiceMock.Verify(bg => bg.QueueUpdateTodoItemAsync(itemId, It.IsAny<Action<TodoItem>>()), Times.Once);
            _userManagerMock.Verify(um => um.UpdateAsync(user), Times.Once);
        }

        #endregion

        #region DeleteTodoItemAsync Tests

        [Test]
        public async Task DeleteTodoItemAsync_UserNotFound_ReturnsFailure()
        {
            // Arrange
            var userId = "non-existing-user";
            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _todoService.DeleteTodoItemAsync(userId, "listId", "itemId");

            // Assert
            result.Success.ShouldBeFalse();
            result.Errors.ShouldContain("No Such User");
        }

        [Test]
        public async Task DeleteTodoItemAsync_ListNotFound_ReturnsFailure()
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
            var result = await _todoService.DeleteTodoItemAsync(userId, "non-existing-list", "itemId");

            // Assert
            result.Success.ShouldBeFalse();
            result.Errors.ShouldContain("No Such List for this User");
        }

        [Test]
        public async Task DeleteTodoItemAsync_ItemFound_RemovesItem()
        {
            // Arrange
            var userId = "existing-user";
            var listId = "existing-list";
            var itemId = "existing-item";
            var todoItem = new TodoItem { Id = itemId };
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

            // Act
            var result = await _todoService.DeleteTodoItemAsync(userId, listId, itemId);

            // Assert
            result.Success.ShouldBeTrue();
            _backgroundJobServiceMock.Verify(bg => bg.QueueDeleteTodoItemAsync(itemId), Times.Once);
            _userManagerMock.Verify(um => um.UpdateAsync(user), Times.Once);
        }

        #endregion

        #region UpdateTodoListOrderIndexAsync Tests

        [Test]
        public async Task UpdateTodoListOrderIndexAsync_UserNotFound_ReturnsFailure()
        {
            // Arrange
            var userId = "non-existing-user";
            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _todoService.UpdateTodoListOrderIndexAsync(userId, "listId", 1);

            // Assert
            result.Success.ShouldBeFalse();
            result.Errors.ShouldContain("No Such User");
        }

        [Test]
        public async Task UpdateTodoListOrderIndexAsync_ListFound_ChangesOrderIndex()
        {
            // Arrange
            var userId = "existing-user";
            var listId = "existing-list";
            var todoList = new TodoList { Id = listId, OrderIndex = 1 };
            var user = new User
            {
                Id = userId,
                TodoLists = new List<TodoList> { todoList }
            };
            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _mapperMock.Setup(m => m.Map<TodoListDto>(todoList))
                .Returns(new TodoListDto { Id = listId, OrderIndex = 2 });

            // Act
            var result = await _todoService.UpdateTodoListOrderIndexAsync(userId, listId, 2);

            // Assert
            result.Success.ShouldBeTrue();
            result.Data.OrderIndex.ShouldBe(2);
            _backgroundJobServiceMock.Verify(bg => bg.QueueUpdateTodoItemAsync(listId, It.IsAny<Action<TodoItem>>()), Times.Once);
            _userManagerMock.Verify(um => um.UpdateAsync(user), Times.Once);
        }

        #endregion

        #region ReorderItems Tests

        [Test]
        public async Task ReorderItems_UserNotFound_ReturnsFailure()
        {
            // Arrange
            var userId = "non-existing-user";
            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _todoService.ReorderItems(userId, "listId", new Dictionary<string, int>());

            // Assert
            result.Success.ShouldBeFalse();
            result.Errors.ShouldContain("No Such User");
        }

        [Test]
        public async Task ReorderItems_ListNotFound_ReturnsFailure()
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
            var result = await _todoService.ReorderItems(userId, "non-existing-list", new Dictionary<string, int>());

            // Assert
            result.Success.ShouldBeFalse();
            result.Errors.ShouldContain("No Such List for this User");
        }

        [Test]
        public async Task ReorderItems_ItemsReorderedSuccessfully_ReturnsSuccess()
        {
            // Arrange
            var userId = "existing-user";
            var listId = "existing-list";
            var todoList = new TodoList
            {
                Id = listId,
                TodoItems = new List<TodoItem>
                {
                    new TodoItem { Id = "item1", OrderIndex = 1 },
                    new TodoItem { Id = "item2", OrderIndex = 2 }
                }
            };
            var user = new User
            {
                Id = userId,
                TodoLists = new List<TodoList> { todoList }
            };
            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync(user);

            var changedItems = new Dictionary<string, int>
            {
                { "item1", 2 },
                { "item2", 1 }
            };

            // Act
            var result = await _todoService.ReorderItems(userId, listId, changedItems);

            // Assert
            result.Success.ShouldBeTrue();
            _userManagerMock.Verify(um => um.UpdateAsync(user), Times.Once);
            _backgroundJobServiceMock.Verify(bg => bg.QueueUpdateTodoItemAsync("item1", It.IsAny<Action<TodoItem>>()), Times.Once);
            _backgroundJobServiceMock.Verify(bg => bg.QueueUpdateTodoItemAsync("item2", It.IsAny<Action<TodoItem>>()), Times.Once);
        }

        #endregion

        #region ReorderList Tests

        [Test]
        public async Task ReorderList_UserNotFound_ReturnsFailure()
        {
            // Arrange
            var userId = "non-existing-user";
            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _todoService.ReorderList(userId, new Dictionary<string, int>());

            // Assert
            result.Success.ShouldBeFalse();
            result.Errors.ShouldContain("No Such User");
        }

        #endregion

        #region GetTodoItemsForListAsync Tests

        [Test]
        public async Task GetTodoItemsForListAsync_UserNotFound_ReturnsEmptyResult()
        {
            // Arrange
            var userId = "non-existing-user";
            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _todoService.GetTodoItemsForListAsync(userId, "listId", 1, 10, default);

            // Assert
            result.Data.ShouldBeEmpty();
        }

        [Test]
        public async Task GetTodoItemsForListAsync_ListNotFound_ReturnsEmptyResult()
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
            var result = await _todoService.GetTodoItemsForListAsync(userId, "non-existing-list", 1, 10, default);

            // Assert
            result.Data.ShouldBeEmpty();
        }

        [Test]
        public async Task GetTodoItemsForListAsync_ListFound_ReturnsTodoItems()
        {
            // Arrange
            var userId = "existing-user";
            var listId = "existing-list";
            var user = new User
            {
                Id = userId,
                TodoLists = new List<TodoList>
                {
                    new TodoList
                    {
                        Id = listId,
                        TodoItems = new List<TodoItem>
                        {
                            new TodoItem { Id = "item1", OrderIndex = 1 },
                            new TodoItem { Id = "item2", OrderIndex = 2 }
                        }
                    }
                }
            };
            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _mapperMock.Setup(m => m.Map<TodoItemDto>(It.IsAny<TodoItem>()))
                .Returns((TodoItem source) => new TodoItemDto
                {
                    Id = source.Id,
                    OrderIndex = source.OrderIndex
                });

            // Act
            var result = await _todoService.GetTodoItemsForListAsync(userId, listId, 1, 10, default);

            // Assert
            result.Data.ShouldNotBeEmpty();
            result.Data.Count.ShouldBe(2);
        }

        #endregion

        [Test]
        public async Task UpdateTodoItemPriorityAsync_ItemFound_UpdatesPriority()
        {
            // Arrange
            var userId = "existing-user";
            var listId = "existing-list";
            var itemId = "existing-item";
            var todoItem = new TodoItem { Id = itemId, Priority = PriorityLevel.Low };
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
                .Returns(new TodoItemDto { Id = itemId, Priority = PriorityLevel.High });

            // Act
            var result = await _todoService.UpdateTodoItemPriorityAsync(userId, listId, itemId, PriorityLevel.High);

            // Assert
            result.Success.ShouldBeTrue();
            result.Data.Priority.ShouldBe(PriorityLevel.High);
            _backgroundJobServiceMock.Verify(bg => bg.QueueUpdateTodoItemAsync(itemId, It.IsAny<Action<TodoItem>>()), Times.Once);
            _userManagerMock.Verify(um => um.UpdateAsync(user), Times.Once);
        }

        [Test]
        public async Task DeleteTodoItemAsync_ItemDeletedSuccessfully()
        {
            // Arrange
            var userId = "existing-user";
            var listId = "existing-list";
            var itemId = "existing-item";
            var todoItem = new TodoItem { Id = itemId };
            var todoList = new TodoList
            {
                Id = listId,
                TodoItems = new List<TodoItem> { todoItem }
            };
            var user = new User
            {
                Id = userId,
                TodoLists = new List<TodoList> { todoList }
            };
            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _todoService.DeleteTodoItemAsync(userId, listId, itemId);

            // Assert
            result.Success.ShouldBeTrue();
            result.Data.ShouldBe(itemId);
            todoList.TodoItems.ShouldBeEmpty();
            _backgroundJobServiceMock.Verify(bg => bg.QueueDeleteTodoItemAsync(itemId), Times.Once);
            _userManagerMock.Verify(um => um.UpdateAsync(user), Times.Once);
        }

        #region CreateTodoListAsync Tests

        [Test]
        public async Task CreateTodoListAsync_UserNotFound_ReturnsFailure()
        {
            // Arrange
            var userId = "non-existing-user";
            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _todoService.CreateTodoListAsync(userId, "New List", "icon", "#ffffff");

            // Assert
            result.Success.ShouldBeFalse();
            result.Errors.ShouldContain("No Such User");
        }

        [Test]
        public async Task CreateTodoListAsync_SuccessfullyCreatesList()
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

            var todoListDto = new TodoListDto { Id = "new-list-id", Title = "New List" };
            _mapperMock.Setup(m => m.Map<TodoListDto>(It.IsAny<TodoList>())).Returns(todoListDto);

            // Act
            var result = await _todoService.CreateTodoListAsync(userId, "New List", "icon", "rgb(36, 142, 255)");

            // Assert
            result.Success.ShouldBeTrue();
            result.Data.Title.ShouldBe("New List");
            user.TodoLists.ShouldHaveSingleItem();
            _userManagerMock.Verify(um => um.UpdateAsync(user), Times.Once);
            _backgroundJobServiceMock.Verify(bg => bg.QueueCreateTodoListAsync(It.IsAny<TodoList>()), Times.Once);
        }

        #endregion

        #region DeleteTodoListAsync Tests

        [Test]
        public async Task DeleteTodoListAsync_UserNotFound_ReturnsFailure()
        {
            // Arrange
            var userId = "non-existing-user";
            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _todoService.DeleteTodoListAsync(userId, "listId");

            // Assert
            result.Success.ShouldBeFalse();
            result.Errors.ShouldContain("No Such User");
        }

        [Test]
        public async Task DeleteTodoListAsync_ListDeletedSuccessfully()
        {
            // Arrange
            var userId = "existing-user";
            var listId = "existing-list";
            var user = new User
            {
                Id = userId,
                TodoLists = new List<TodoList>
                {
                    new TodoList { Id = listId }
                }
            };
            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _todoService.DeleteTodoListAsync(userId, listId);

            // Assert
            result.Success.ShouldBeTrue();
            result.Data.ShouldBe(listId);
            user.TodoLists.ShouldBeEmpty();
            _backgroundJobServiceMock.Verify(bg => bg.QueueDeleteTodoListAsync(listId), Times.Once);
            _userManagerMock.Verify(um => um.UpdateAsync(user), Times.Once);
        }

        #endregion

        #region UpdateTodoListTitleAsync Tests

        [Test]
        public async Task UpdateTodoListTitleAsync_UserNotFound_ReturnsFailure()
        {
            // Arrange
            var userId = "non-existing-user";
            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _todoService.UpdateTodoListTitleAsync(userId, "listId", "New Title");

            // Assert
            result.Success.ShouldBeFalse();
            result.Errors.ShouldContain("No Such User");
        }

        [Test]
        public async Task UpdateTodoListTitleAsync_ListNotFound_ReturnsFailure()
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
            var result = await _todoService.UpdateTodoListTitleAsync(userId, "non-existing-list", "New Title");

            // Assert
            result.Success.ShouldBeFalse();
            result.Errors.ShouldContain("No Such List for this User");
        }

        [Test]
        public async Task UpdateTodoListTitleAsync_SuccessfullyUpdatesTitle()
        {
            // Arrange
            var userId = "existing-user";
            var listId = "existing-list";
            var user = new User
            {
                Id = userId,
                TodoLists = new List<TodoList>
                {
                    new TodoList { Id = listId, Title = "Old Title" }
                }
            };
            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync(user);

            var todoListDto = new TodoListDto { Id = listId, Title = "New Title" };
            _mapperMock.Setup(m => m.Map<TodoListDto>(It.IsAny<TodoList>())).Returns(todoListDto);

            // Act
            var result = await _todoService.UpdateTodoListTitleAsync(userId, listId, "New Title");

            // Assert
            result.Success.ShouldBeTrue();
            result.Data.Title.ShouldBe("New Title");
            _userManagerMock.Verify(um => um.UpdateAsync(user), Times.Once);
            _backgroundJobServiceMock.Verify(bg => bg.QueueUpdateTodoListAsync(listId, It.IsAny<Action<TodoList>>()), Times.Once);
        }

        #endregion

        #region UpdateTodoListColorAsync Tests

        [Test]
        public async Task UpdateTodoListColorAsync_UserNotFound_ReturnsFailure()
        {
            // Arrange
            var userId = "non-existing-user";
            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _todoService.UpdateTodoListColorAsync(userId, "listId", "#ffffff");

            // Assert
            result.Success.ShouldBeFalse();
            result.Errors.ShouldContain("No Such User");
        }

        [Test]
        public async Task UpdateTodoListColorAsync_ListNotFound_ReturnsFailure()
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
            var result = await _todoService.UpdateTodoListColorAsync(userId, "non-existing-list", "#ffffff");

            // Assert
            result.Success.ShouldBeFalse();
            result.Errors.ShouldContain("No Such List for this User");
        }

        [Test]
        public async Task UpdateTodoListColorAsync_SuccessfullyUpdatesColor()
        {
            // Arrange
            var userId = "existing-user";
            var listId = "existing-list";
            var user = new User
            {
                Id = userId,
                TodoLists = new List<TodoList>
                {
                    new TodoList { Id = listId, Color = Color.From("rgb(232, 67, 254)") }
                }
            };
            _userManagerMock.Setup(um => um.FindByIdAsync(userId))
                .ReturnsAsync(user);

            var todoListDto = new TodoListDto { Id = listId, Color = Color.From("rgb(36, 142, 255)") };
            _mapperMock.Setup(m => m.Map<TodoListDto>(It.IsAny<TodoList>())).Returns(todoListDto);

            // Act
            var result = await _todoService.UpdateTodoListColorAsync(userId, listId, "rgb(36, 142, 255)");

            // Assert
            result.Success.ShouldBeTrue();
            result.Data.Color.ShouldBe(Color.From("rgb(36, 142, 255)"));
            _userManagerMock.Verify(um => um.UpdateAsync(user), Times.Once);
            _backgroundJobServiceMock.Verify(bg => bg.QueueUpdateTodoListAsync(listId, It.IsAny<Action<TodoList>>()), Times.Once);
        }

        #endregion
    }
}

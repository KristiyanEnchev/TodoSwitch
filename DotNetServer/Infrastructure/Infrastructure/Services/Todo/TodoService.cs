namespace Infrastructure.Services.Todo
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Identity;

    using AutoMapper;

    using MongoDB.Bson;

    using Domain.Entities;

    using Models.Todo;

    using Shared;

    using Application.Extensions;
    using Application.Interfaces.Services;

    public class TodoService : ITodoService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> userManager;
        private readonly ITodoBackgroundJobService _backgroundJobService;

        public TodoService(IMapper mapper, UserManager<User> userManager, ITodoBackgroundJobService backgroundJobService)
        {
            _mapper = mapper;
            this.userManager = userManager;
            _backgroundJobService = backgroundJobService;
        }

        public async Task<Result<List<GetUserTodoListsDto>>> GetUserTodoListsAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Result<List<GetUserTodoListsDto>>.Failure("No Such User");
            }

            var list = user.TodoLists.Select(item => _mapper.Map<GetUserTodoListsDto>(item)).ToList();

            return Result<List<GetUserTodoListsDto>>.SuccessResult(list);
        }

        public async Task<Result<GetUserTodoItemDto>> GetTodoItemByIdAsync(string userId, string listId, string itemId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<GetUserTodoItemDto>.Failure("No Such User");
            }

            var todoList = user.TodoLists.FirstOrDefault(tl => tl.Id == listId);
            if (todoList == null)
            {
                return Result<GetUserTodoItemDto>.Failure("No Such List for this User");
            }

            var todoItem = todoList.TodoItems.FirstOrDefault(x => x.Id == itemId);
            if (todoItem == null)
            {
                return Result<GetUserTodoItemDto>.Failure("No Such Item for this List");
            }

            var result = _mapper.Map<GetUserTodoItemDto>(todoItem);
            return Result<GetUserTodoItemDto>.SuccessResult(result);
        }

        public async Task<PaginatedResult<TodoItemDto>> GetTodoItemsForListAsync(
            string userId,
            string todoListId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return PaginatedResult<TodoItemDto>.Create(new List<TodoItemDto>(), 0, pageNumber, pageSize);
            }

            var todoList = user.TodoLists.FirstOrDefault(list => list.Id == todoListId);
            if (todoList == null)
            {
                return PaginatedResult<TodoItemDto>.Create(new List<TodoItemDto>(), 0, pageNumber, pageSize);
            }

            var paginatedTodoItems = await todoList.TodoItems.AsQueryable()
                .OrderBy(item => item.OrderIndex)
                .ToPaginatedListAsync(pageNumber, pageSize, item => _mapper.Map<TodoItemDto>(item), cancellationToken);

            return paginatedTodoItems;
        }

        // Method to toggle the Done status of a TodoItem for a specific user list
        public async Task<Result<TodoItemDto>> ToggleTodoItemDoneStatusAsync(string userId, string todoListId, string todoItemId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<TodoItemDto>.Failure("No Such User");
            }

            var todoList = user.TodoLists.FirstOrDefault(tl => tl.Id == todoListId);
            if (todoList == null)
            {
                return Result<TodoItemDto>.Failure("No Such List for this User");
            }

            var todoItem = todoList.TodoItems.FirstOrDefault(ti => ti.Id == todoItemId);
            if (todoItem == null)
            {
                return Result<TodoItemDto>.Failure("No Such Item for this List");
            }

            todoItem.IsDone = !todoItem.IsDone;
            await userManager.UpdateAsync(user);

            var todoDto = _mapper.Map<TodoItemDto>(todoItem);

            await _backgroundJobService.QueueUpdateTodoItemAsync(todoItemId, item => item.IsDone = !item.IsDone);

            return Result<TodoItemDto>.SuccessResult(todoDto);
        }

        // Method to change the Priority of a TodoItem for a user list
        public async Task<Result<TodoItemDto>> UpdateTodoItemPriorityAsync(string userId, string todoListId, string todoItemId, PriorityLevel newPriority)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<TodoItemDto>.Failure("No Such User");
            }

            var todoList = user.TodoLists.FirstOrDefault(tl => tl.Id == todoListId);
            if (todoList == null)
            {
                return Result<TodoItemDto>.Failure("No Such List for this User");
            }

            var todoItem = todoList.TodoItems.FirstOrDefault(ti => ti.Id == todoItemId);
            if (todoItem == null)
            {
                return Result<TodoItemDto>.Failure("No Such Item for this List");
            }

            todoItem.Priority = newPriority;
            await userManager.UpdateAsync(user);

            var todoDto = _mapper.Map<TodoItemDto>(todoItem);

            await _backgroundJobService.QueueUpdateTodoItemAsync(todoItemId, item => item.Priority = newPriority);

            return Result<TodoItemDto>.SuccessResult(todoDto);
        }

        // Method to change the order index of a TodoItem for a specific user's list
        public async Task<Result<TodoItemDto>> UpdateTodoItemOrderIndexAsync(string userId, string todoListId, string todoItemId, int newOrderIndex)
        {
            var todo = new TodoItemDto();

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<TodoItemDto>.Failure("No Such User");
            }

            var todoList = user.TodoLists.FirstOrDefault(tl => tl.Id == todoListId);
            if (todoList == null)
            {
                return Result<TodoItemDto>.Failure("No Such List for this User");
            }

            var todoItem = todoList.TodoItems.FirstOrDefault(ti => ti.Id == todoItemId);
            if (todoItem == null)
            {
                return Result<TodoItemDto>.Failure("No Such Item for this List");
            }

            int maxIndex = todoList.TodoItems.Count - 1;
            int adjustedOrderIndex = Math.Max(0, Math.Min(newOrderIndex, maxIndex));

            todoList.TodoItems.Remove(todoItem);
            todoList.TodoItems.Insert(adjustedOrderIndex, todoItem);

            for (int i = 0; i < todoList.TodoItems.Count; i++)
            {
                todoList.TodoItems[i].OrderIndex = i;
            }

            await userManager.UpdateAsync(user);

            var batchTasks = todoList.TodoItems.Select(item =>
                _backgroundJobService.QueueUpdateTodoItemAsync(item.Id, updatedItem =>
                {
                    updatedItem.OrderIndex = item.OrderIndex;
                })
            );

            await Task.WhenAll(batchTasks);

            todo = _mapper.Map<TodoItemDto>(todoItem);

            return Result<TodoItemDto>.SuccessResult(todo);
        }

        // Method to create a TodoItem
        public async Task<Result<TodoItemDto>> CreateTodoItemAsync(string userId, CreateTodoItemDto todoItemDto)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<TodoItemDto>.Failure("No Such User");
            }

            var todoList = user.TodoLists.FirstOrDefault(tl => tl.Id == todoItemDto.ListId);
            if (todoList == null)
            {
                return Result<TodoItemDto>.Failure("No Such List for this User");
            }

            var todoItem = _mapper.Map<TodoItem>(todoItemDto);

            todoItem.Id = ObjectId.GenerateNewId().ToString();
            todoItem.OrderIndex = todoList.TodoItems.Count();
            todoItem.IsDone = false;
            todoList.TodoItems.Add(todoItem);

            await userManager.UpdateAsync(user);

            var todoDto = _mapper.Map<TodoItemDto>(todoItem);

            await _backgroundJobService.QueueCreateTodoItemAsync(todoItem);

            return Result<TodoItemDto>.SuccessResult(todoDto);
        }

        // Method to delete a TodoItem
        public async Task<Result<string>> DeleteTodoItemAsync(string userId, string todoListId, string todoItemId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<string>.Failure("No Such User");
            }

            var todoList = user.TodoLists.FirstOrDefault(tl => tl.Id == todoListId);
            if (todoList == null)
            {
                return Result<string>.Failure("No Such List for this User");
            }

            var todoItem = todoList.TodoItems.FirstOrDefault(ti => ti.Id == todoItemId);
            if (todoItem == null)
            {
                return Result<string>.Failure("No Such Item for this List");
            }

            todoList.TodoItems.Remove(todoItem);
            await userManager.UpdateAsync(user);

            await _backgroundJobService.QueueDeleteTodoItemAsync(todoItem.Id);

            return Result<string>.SuccessResult(todoItem.Id);
        }

        // Method to update a TodoItem Descriptions
        public async Task<Result<TodoItemDto>> UpdateTodoItemDescriptionAsync(string userId, string todoListId, string itemId, string title, string note)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<TodoItemDto>.Failure("No Such User");
            }

            var todoList = user.TodoLists.FirstOrDefault(tl => tl.Id == todoListId);
            if (todoList == null)
            {
                return Result<TodoItemDto>.Failure("No Such List for this User");
            }

            var todoItem = todoList.TodoItems.FirstOrDefault(ti => ti.Id == itemId);
            if (todoItem == null)
            {
                return Result<TodoItemDto>.Failure("No Such Item for this List");
            }

            if (!string.IsNullOrEmpty(title))
            {
                todoItem.Title = title;
                await _backgroundJobService.QueueUpdateTodoItemAsync(todoItem.Id, item => item.Title = title);
            }

            if (!string.IsNullOrEmpty(note))
            {
                todoItem.Note = note;
                await _backgroundJobService.QueueUpdateTodoItemAsync(todoItem.Id, item => item.Note = note);
            }

            await userManager.UpdateAsync(user);

            var todoDto = _mapper.Map<TodoItemDto>(todoItem);

            return Result<TodoItemDto>.SuccessResult(todoDto);
        }

        // Method to create a TodoList with an empty TodoItem list
        public async Task<Result<TodoListDto>> CreateTodoListAsync(string userId, string title, string colorCode)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<TodoListDto>.Failure("No Such User");
            }

            var todoList = new TodoList
            {
                Id = ObjectId.GenerateNewId().ToString(),
                UserId = userId,
                Title = title,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                Color = Color.From(colorCode),
                TodoItems = new List<TodoItem>()
            };

            user.TodoLists.Add(todoList);
            await userManager.UpdateAsync(user);

            var list = _mapper.Map<TodoListDto>(todoList);

            await _backgroundJobService.QueueCreateTodoListAsync(todoList);

            return Result<TodoListDto>.SuccessResult(list);
        }

        // Method to delete a TodoList
        public async Task<Result<string>> DeleteTodoListAsync(string userId, string todoListId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<string>.Failure("No Such User");
            }

            user.TodoLists.RemoveAll(tl => tl.Id == todoListId);
            await userManager.UpdateAsync(user);

            await _backgroundJobService.QueueDeleteTodoListAsync(todoListId);

            return Result<string>.SuccessResult(todoListId);
        }

        // Method to update a TodoList Title
        public async Task<Result<TodoListDto>> UpdateTodoListTitleAsync(string userId, string listId, string title)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<TodoListDto>.Failure("No Such User");
            }

            var todoList = user.TodoLists.FirstOrDefault(tl => tl.Id == listId);
            if (todoList == null)
            {
                return Result<TodoListDto>.Failure("No Such List for this User");
            }

            todoList.Title = title;
            await userManager.UpdateAsync(user);

            var list = _mapper.Map<TodoListDto>(todoList);

            await _backgroundJobService.QueueUpdateTodoListAsync(listId, tl => tl.Title = title);

            return Result<TodoListDto>.SuccessResult(list);
        }

        // Method to update a TodoList Color
        public async Task<Result<TodoListDto>> UpdateTodoListColorAsync(string userId, string todoListId, string colorCode)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<TodoListDto>.Failure("No Such User");
            }

            var todoList = user.TodoLists.FirstOrDefault(tl => tl.Id == todoListId);
            if (todoList == null)
            {
                return Result<TodoListDto>.Failure("No Such List for this User");
            }

            todoList.Color = Color.From(colorCode);
            await userManager.UpdateAsync(user);

            var list = _mapper.Map<TodoListDto>(todoList);

            await _backgroundJobService.QueueUpdateTodoListAsync(todoListId, tl => tl.Color = Color.From(colorCode));

            return Result<TodoListDto>.SuccessResult(list);
        }

        // Method to get all TodoList Colors
        public async Task<List<SupportedColorDto>> GetAllSupportedListColors()
        {
            var supportedColors = new List<SupportedColorDto>();

            foreach (var color in Color.SupportedColors)
            {
                supportedColors.Add(new SupportedColorDto
                {
                    Name = color.Name,
                    Code = color.Code
                });
            }

            return supportedColors;
        }
    }
}
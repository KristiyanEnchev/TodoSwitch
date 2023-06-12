namespace Infrastructure.Services.Todo
{
    using System.Collections.Generic;

    using Microsoft.Extensions.Caching.Memory;

    using Application.Interfaces.Services;

    using Models.Todo;

    using Shared;

    using Domain.Entities;

    public class CachedTodoService : ICachedTodoService
    {
        private readonly ITodoService _todoService;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

        public CachedTodoService(ITodoService todoService, IMemoryCache cache)
        {
            _todoService = todoService;
            _cache = cache;
        }

        public async Task<Result<List<GetUserTodoListsDto>>> GetUserTodoListsAsync(string userId)
        {
            string cacheKey = $"UserTodoLists_{userId}";
            if (!_cache.TryGetValue(cacheKey, out Result<List<GetUserTodoListsDto>> userTodoLists))
            {
                userTodoLists = await _todoService.GetUserTodoListsAsync(userId);
                if (userTodoLists != null && userTodoLists.Success && userTodoLists.Data != null)
                {
                    _cache.Set(cacheKey, userTodoLists, _cacheDuration);
                }
            }
            return userTodoLists;
        }

        public async Task<Result<GetUserTodoItemDto>> GetTodoItemByIdAsync(string userId, string listId, string itemId)
        {
            string cacheKey = $"TodoItem_{userId}_{listId}_{itemId}";
            if (!_cache.TryGetValue(cacheKey, out Result<GetUserTodoItemDto> todoItem))
            {
                todoItem = await _todoService.GetTodoItemByIdAsync(userId, listId, itemId);
                if (todoItem != null && todoItem.Success && todoItem.Data != null)
                {
                    _cache.Set(cacheKey, todoItem, _cacheDuration);
                }
            }
            return todoItem;
        }

        public async Task<PaginatedResult<TodoItemDto>> GetTodoItemsForListAsync(string userId, string todoListId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            string cacheKey = $"TodoItem_{userId}_{todoListId}";
            if (!_cache.TryGetValue(cacheKey, out PaginatedResult<TodoItemDto> paginatedTodoItems))
            {
                paginatedTodoItems = await _todoService.GetTodoItemsForListAsync(userId, todoListId, pageNumber, pageSize, cancellationToken);
                if (paginatedTodoItems != null)
                {
                    _cache.Set(cacheKey, paginatedTodoItems, _cacheDuration);
                }
            }
            return paginatedTodoItems;
        }

        public async Task<Result<TodoItemDto>> ToggleTodoItemDoneStatusAsync(string userId, string todoListId, string todoItemId)
        {
            var todoItemDto = await _todoService.ToggleTodoItemDoneStatusAsync(userId, todoListId, todoItemId);
            if (todoItemDto != null && todoItemDto.Success && todoItemDto.Data != null)
            {
                InvalidateTodoItemCache(userId, todoListId, todoItemId);
            }
            return todoItemDto;
        }

        public async Task<Result<TodoItemDto>> UpdateTodoItemPriorityAsync(string userId, string todoListId, string todoItemId, PriorityLevel newPriority)
        {
            var todoItemDto = await _todoService.UpdateTodoItemPriorityAsync(userId, todoListId, todoItemId, newPriority);
            if (todoItemDto != null && todoItemDto.Success && todoItemDto.Data != null)
            {
                InvalidateTodoItemCache(userId, todoListId, todoItemId);
            }
            return todoItemDto;
        }

        public async Task<Result<TodoItemDto>> UpdateTodoItemOrderIndexAsync(string userId, string todoListId, string todoItemId, int newOrderIndex)
        {
            var todoItemDto = await _todoService.UpdateTodoItemOrderIndexAsync(userId, todoListId, todoItemId, newOrderIndex);
            if (todoItemDto != null && todoItemDto.Success && todoItemDto.Data != null)
            {
                InvalidateTodoItemCache(userId, todoListId, todoItemId);
            }
            return todoItemDto;
        }

        public async Task<Result<TodoItemDto>> CreateTodoItemAsync(string userId, CreateTodoItemDto todoItemDto)
        {
            var createdItem = await _todoService.CreateTodoItemAsync(userId, todoItemDto);
            if (createdItem != null && createdItem.Success && createdItem.Data != null)
            {
                InvalidateUserTodoListsCache(userId);
                InvalidateTodoItemCache(userId, todoItemDto.ListId, createdItem.Data.Id);
            }
            return createdItem;
        }

        public async Task<Result<string>> DeleteTodoItemAsync(string userId, string todoListId, string todoItemId)
        {
            var deletedItemId = await _todoService.DeleteTodoItemAsync(userId, todoListId, todoItemId);
            if (deletedItemId != null && deletedItemId.Success && !string.IsNullOrEmpty(deletedItemId.Data))
            {
                InvalidateUserTodoListsCache(userId);
                InvalidateTodoItemCache(userId, todoListId, todoItemId);
            }
            return deletedItemId;
        }

        public async Task<Result<TodoItemDto>> UpdateTodoItemDescriptionAsync(string userId, string todoListId, string itemId, string title, string note)
        {
            var updatedItem = await _todoService.UpdateTodoItemDescriptionAsync(userId, todoListId, itemId, title, note);
            if (updatedItem != null && updatedItem.Success && updatedItem.Data != null)
            {
                InvalidateTodoItemCache(userId, todoListId, itemId);
            }
            return updatedItem;
        }

        public async Task<Result<TodoListDto>> CreateTodoListAsync(string userId, string title, string colorCode)
        {
            var createdList = await _todoService.CreateTodoListAsync(userId, title, colorCode);
            if (createdList != null && createdList.Success && createdList.Data != null)
            {
                InvalidateUserTodoListsCache(userId);
            }
            return createdList;
        }

        public async Task<Result<string>> DeleteTodoListAsync(string userId, string todoListId)
        {
            var deletedListId = await _todoService.DeleteTodoListAsync(userId, todoListId);
            if (deletedListId != null && deletedListId.Success && !string.IsNullOrEmpty(deletedListId.Data))
            {
                InvalidateUserTodoListsCache(userId);
            }
            return deletedListId;
        }

        public async Task<Result<TodoListDto>> UpdateTodoListTitleAsync(string userId, string listId, string title)
        {
            var updatedList = await _todoService.UpdateTodoListTitleAsync(userId, listId, title);
            if (updatedList != null && updatedList.Success && updatedList.Data != null)
            {
                InvalidateUserTodoListsCache(userId);
            }
            return updatedList;
        }

        public async Task<Result<TodoListDto>> UpdateTodoListColorAsync(string userId, string todoListId, string colorCode)
        {
            var updatedList = await _todoService.UpdateTodoListColorAsync(userId, todoListId, colorCode);
            if (updatedList != null && updatedList.Success && updatedList.Data != null)
            {
                InvalidateUserTodoListsCache(userId);
            }
            return updatedList;
        }

        public async Task<List<SupportedColorDto>> GetAllSupportedListColors()
        {
            string cacheKey = $"SupportedColors";
            if (!_cache.TryGetValue(cacheKey, out List<SupportedColorDto> supportedColors))
            {
                supportedColors = await _todoService.GetAllSupportedListColors();

                _cache.Set(cacheKey, supportedColors, _cacheDuration);
            }

            return supportedColors;
        }

        private void InvalidateUserTodoListsCache(string userId)
        {
            _cache.Remove($"UserTodoLists_{userId}");
        }

        private void InvalidateTodoItemCache(string userId, string listId, string itemId)
        {
            _cache.Remove($"TodoItem_{userId}_{listId}_{itemId}");
            _cache.Remove($"TodoItem_{userId}_{listId}");
            _cache.Remove($"TodoItem_{listId}_{itemId}");
        }
    }
}
namespace Infrastructure.Services.Todo
{
    using System.Threading;

    using AutoMapper;

    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    using Application.Extensions;
    using Application.Interfaces.Services;

    using Domain.Entities;

    using Models;
    using Models.Todo;

    using Persistence.Repository;

    using Shared;

    public class TodoService : ITodoService
    {
        private readonly IMongoRepository<TodoList> _todoListRepo;
        private readonly IMongoRepository<TodoItem> _todoItemRepo;
        private readonly IMapper _mapper;

        public TodoService(IMongoRepository<TodoList> todoListRepo, IMapper mapper, IMongoRepository<TodoItem> todoItemRepo)
        {
            _todoListRepo = todoListRepo;
            _mapper = mapper;
            _todoItemRepo = todoItemRepo;
        }

        public async Task<Result<TodosVm>> GetTodos()
        {
            var todoList = _todoListRepo.Entities.OrderBy(t => t.Title).ToList();
            var todoListDto = _mapper.ProjectTo<TodoListDto>(todoList.AsQueryable()).ToList();

            var model = new TodosVm
            {
                PriorityLevels = Enum.GetValues(typeof(PriorityLevel))
                    .Cast<PriorityLevel>()
                    .Select(p => new LookupDto { Id = (int)p, Title = p.ToString() })
                    .ToList(),

                Lists = todoListDto
            };

            return Result<TodosVm>.SuccessResult(model);
        }

        public async Task<PaginatedResult<TodoList>> GetPagedTodos(
            bool ascending,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            return await _todoListRepo.Entities
                .Sort(x => x.Id, ascending)
                .ToPaginatedListAsync(pageNumber, pageSize, cancellationToken);
        }

        public async Task<PaginatedResult<TodoListDto>> GetPagedTodosForUser(
            bool ascending,
            int pageNumber,
            int pageSize,
            string userId,
            CancellationToken cancellationToken)
        {
            var todoList = await _todoListRepo.Entities
                .Where(x => x.UserId == userId)
                .Sort(x => x.Id, ascending)
                .ToPaginatedListAsync(pageNumber, pageSize,
                    entity => _mapper.Map<TodoListDto>(entity),
                    cancellationToken);

            return todoList;
        }

        public async Task<TodoItemDto> ToggleDoneTodoItem(string listId, string itemId)
        {
            var todoItem = await _todoItemRepo.Entities
                .Where(x => x.ListId == listId && x.Id == itemId)
                .FirstOrDefaultAsync()
                ?? throw new Exception("Todo item not found");

            todoItem.Done = !todoItem.Done;

            await _todoItemRepo.UpdateAsync(todoItem);

            var todoItemDto = _mapper.Map<TodoItemDto>(todoItem);

            return todoItemDto;
        }

        public async Task<TodoItemDto> ChangePriorityTodoItem(string listId, string itemId, int newPriority)
        {
            var todoItem = await _todoItemRepo.Entities
                .Where(x => x.ListId == listId && x.Id == itemId)
                .FirstOrDefaultAsync()
                ?? throw new Exception("Todo item not found");

            todoItem.Priority = (PriorityLevel)newPriority;

            await _todoItemRepo.UpdateAsync(todoItem);

            var todoItemDto = _mapper.Map<TodoItemDto>(todoItem);

            return todoItemDto;
        }

        public async Task<TodoItemDto> ChangeOrderTodoItem(string listId, string itemId, int newOrder)
        {
            var todoItem = await _todoItemRepo.Entities
                .Where(x => x.ListId == listId && x.Id == itemId)
                .FirstOrDefaultAsync()
                ?? throw new Exception("Todo item not found");

            todoItem.OrderIndex = newOrder;

            await _todoItemRepo.UpdateAsync(todoItem);

            var todoItemDto = _mapper.Map<TodoItemDto>(todoItem);

            return todoItemDto;
        }
    }
}
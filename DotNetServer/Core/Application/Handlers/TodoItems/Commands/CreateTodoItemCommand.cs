namespace Application.Handlers.TodoItems.Commands
{
    using MediatR;

    using Models.Todo;

    using Application.Interfaces.Services;

    using Shared;

    public record CreateTodoItemCommand : IRequest<Result<TodoItemDto>>
    {
        public string UserId { get; init; }
        public CreateTodoItemDto Todo { get; init; }

        public CreateTodoItemCommand(string userId, CreateTodoItemDto todo)
        {
            UserId = userId;
            Todo = todo;
        }

        public class CreateTodoItemCommandHandler : IRequestHandler<CreateTodoItemCommand, Result<TodoItemDto>>
        {
            private readonly ICachedTodoService _todoService;

            public CreateTodoItemCommandHandler(ICachedTodoService todoService)
            {
                _todoService = todoService;
            }

            public async Task<Result<TodoItemDto>> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
            {
                var result = await _todoService.CreateTodoItemAsync(request.UserId, request.Todo);

                return result;
            }
        }
    }
}
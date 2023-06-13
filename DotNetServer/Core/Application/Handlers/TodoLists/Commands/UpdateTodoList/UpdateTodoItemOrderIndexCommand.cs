namespace Application.Handlers.TodoLists.Commands.UpdateTodoList
{
    using MediatR;

    using Models.Todo;

    using Application.Interfaces.Services;

    using Shared;

    public record UpdateTodoListOrderIndexCommand : IRequest<Result<TodoListDto>>
    {
        public string UserId { get; init; }
        public string ListId { get; init; }
        public int NewOrderIndex { get; }

        public UpdateTodoListOrderIndexCommand(string userId, string listId, int newOrderIndex)
        {
            UserId = userId;
            ListId = listId;
            NewOrderIndex = newOrderIndex;
        }

        public class UpdateTodoListOrderIndexCommandHandler : IRequestHandler<UpdateTodoListOrderIndexCommand, Result<TodoListDto>>
        {
            private readonly ICachedTodoService _todoService;

            public UpdateTodoListOrderIndexCommandHandler(ICachedTodoService todoService)
            {
                _todoService = todoService;
            }

            public async Task<Result<TodoListDto>> Handle(UpdateTodoListOrderIndexCommand request, CancellationToken cancellationToken)
            {
                var result = await _todoService.UpdateTodoListOrderIndexAsync(request.UserId, request.ListId, request.NewOrderIndex);

                return result;
            }
        }
    }
}
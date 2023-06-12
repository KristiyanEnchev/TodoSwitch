namespace Application.Handlers.TodoLists.Commands.UpdateTodoList
{
    using MediatR;

    using Models.Todo;

    using Application.Interfaces.Services;

    using Shared;

    public record UpdateListColorCommand : IRequest<Result<TodoListDto>>
    {
        public string UserId { get; init; }
        public string ListId { get; init; }
        public string ColorCode { get; init; }

        public UpdateListColorCommand(string userId, string listId, string colorCode)
        {
            UserId = userId;
            ListId = listId;
            ColorCode = colorCode;
        }

        public class UpdateListColorCommandHandler : IRequestHandler<UpdateListColorCommand, Result<TodoListDto>>
        {
            private readonly ICachedTodoService _todoService;

            public UpdateListColorCommandHandler(ICachedTodoService todoService)
            {
                _todoService = todoService;
            }

            public async Task<Result<TodoListDto>> Handle(UpdateListColorCommand request, CancellationToken cancellationToken) 
            {
                var result = await _todoService.UpdateTodoListColorAsync(request.UserId, request.ListId, request.ColorCode);

                return result;
            }
        }
    }
}
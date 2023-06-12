namespace Application.Handlers.TodoLists.Commands.DeleteList
{
    using MediatR;

    using Application.Interfaces.Services;

    using Shared;

    public record DeleteListCommand : IRequest<Result<string>>
    {
        public string UserId { get; init; }
        public string ListId { get; init; }

        public DeleteListCommand(string userId, string listId)
        {
            UserId = userId;
            ListId = listId;
        }

        public class DeleteListCommandHandler : IRequestHandler<DeleteListCommand, Result<string>> 
        {
            private readonly ICachedTodoService _service;

            public DeleteListCommandHandler(ICachedTodoService service)
            {
                _service = service;
            }

            public async Task<Result<string>> Handle(DeleteListCommand request, CancellationToken cancellationToken) 
            {
                var result = await _service.DeleteTodoListAsync(request.UserId, request.ListId);

                return result;
            }
        }
    }
}
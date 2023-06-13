namespace Application.Handlers.TodoLists.Commands.UpdateTodoList
{
    using MediatR;

    using Models.Todo;

    using Application.Interfaces.Services;

    using Shared;

    public record UpdateListOrderIndexCommand : IRequest<Result<List<GetUserTodoListsDto>>>
    {
        public string UserId { get; init; }
        public Dictionary<string, int> ChangedList { get; }

        public UpdateListOrderIndexCommand(string userId, Dictionary<string, int> changedList)
        {
            UserId = userId;
            ChangedList = changedList;
        }

        public class UpdateListOrderIndexCommandHandler : IRequestHandler<UpdateListOrderIndexCommand, Result<List<GetUserTodoListsDto>>>
        {
            private readonly ICachedTodoService _todoService;

            public UpdateListOrderIndexCommandHandler(ICachedTodoService todoService)
            {
                _todoService = todoService;
            }

            public async Task<Result<List<GetUserTodoListsDto>>> Handle(UpdateListOrderIndexCommand request, CancellationToken cancellationToken)
            {
                var result = await _todoService.ReorderList(request.UserId, request.ChangedList);

                return result;
            }
        }
    }
}
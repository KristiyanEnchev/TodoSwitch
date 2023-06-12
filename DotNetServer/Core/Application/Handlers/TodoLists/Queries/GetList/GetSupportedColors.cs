namespace Application.Handlers.TodoLists.Queries.GetList
{
    using MediatR;

    using Application.Interfaces.Services;

    using Models.Todo;

    public record GetSupportedColorsQuery : IRequest<List<SupportedColorDto>>
    {

        public class GetSupportedColorsQueryHandler : IRequestHandler<GetSupportedColorsQuery, List<SupportedColorDto>>
        {
            private readonly ICachedTodoService _service;
            public GetSupportedColorsQueryHandler(ICachedTodoService service)
            {
                _service = service;
            }

            public async Task<List<SupportedColorDto>> Handle(GetSupportedColorsQuery request, CancellationToken cancellationToken)
            {
                var result = await _service.GetAllSupportedListColors();

                return result;
            }
        }
    }
}
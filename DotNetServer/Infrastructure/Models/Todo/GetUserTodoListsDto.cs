namespace Models.Todo
{
    using AutoMapper;

    using Domain.Entities;

    using Shared.Mappings;

    public class GetUserTodoListsDto : IMapFrom<TodoList>
    {
        public string Id { get; init; }
        public string Title { get; set; }
        public SupportedColorDto Color { get; set; }

        public virtual void Mapping(Profile mapper)
        {
            mapper.CreateMap<GetUserTodoListsDto, TodoList>().ReverseMap();
            mapper.CreateMap<TodoList, GetUserTodoListsDto>()
             .ForMember(dest => dest.Color, opt => opt.MapFrom(src => new SupportedColorDto { Name = src.Color.Name, Code = src.Color.Code }))
             .ReverseMap();
        }
    }
}
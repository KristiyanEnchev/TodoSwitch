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
        public int OrderIndex { get; set; }
        public string Icon { get; set; }
        public string CompletedCount { get; set; }
        public string TaskCounts { get; set; }

        public virtual void Mapping(Profile mapper)
        {

            mapper.CreateMap<TodoList, GetUserTodoListsDto>()
             .ForMember(dest => dest.Color, opt => opt.MapFrom(src => new SupportedColorDto { Name = src.Color.Name, Code = src.Color.Code }))
             .ForMember(dest => dest.CompletedCount, opt => opt.MapFrom(src => src.TodoItems.Count(item => item.IsDone).ToString()))
             .ForMember(dest => dest.TaskCounts, opt => opt.MapFrom(src => src.TodoItems.Count.ToString()))
             .ReverseMap();
        }
    }
}
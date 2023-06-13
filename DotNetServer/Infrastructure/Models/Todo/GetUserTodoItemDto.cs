namespace Models.Todo
{
    using AutoMapper;

    using Domain.Entities;

    using Shared.Mappings;

    public class GetUserTodoItemDto : IMapFrom<TodoItem>
    {
        public string ListId { get; set; }
        public string Title { get; init; }
        public string Note { get; init; }
        public PriorityLevel Priority { get; init; }
        public int OrderIndex { get; init; }
        public bool IsDone { get; init; }
        public string Reminder { get; set; }
        public string CreatedOn { get; set; }

        public virtual void Mapping(Profile mapper)
        {
            mapper.CreateMap<GetUserTodoItemDto, TodoItem>()
             .ForMember(dest => dest.Reminder, opt => opt.MapFrom(src => DateTime.Parse(src.Reminder).ToLocalTime()))
             .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.Parse(src.CreatedOn).ToLocalTime()))
             .ReverseMap();
        }
    }
}
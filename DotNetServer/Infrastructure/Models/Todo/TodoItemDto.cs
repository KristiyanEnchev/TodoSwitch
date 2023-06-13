namespace Models.Todo
{
    using System;

    using AutoMapper;

    using Domain.Entities;

    using Shared.Mappings;

    public class TodoItemDto : IMapFrom<TodoItem>
    {
        public string? Id { get; init; }
        public string? Title { get; init; }
        public string? Note { get; init; }
        public PriorityLevel Priority { get; init; }
        public int OrderIndex { get; init; }
        public bool IsDone { get; init; }
        public string Reminder { get; init; } 
        public string CreatedOn { get; init; }

        public virtual void Mapping(Profile mapper)
        {
            mapper.CreateMap<TodoItemDto, TodoItem>()
                .ForMember(dest => dest.Reminder, opt => opt.MapFrom(src => DateTime.Parse(src.Reminder).ToLocalTime()))
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.Parse(src.CreatedOn).ToLocalTime()))
                .ReverseMap();

            mapper.CreateMap<TodoItem, TodoItemDto>()
              .ForMember(dest => dest.Reminder, opt => opt.MapFrom(src => src.Reminder.ToLocalTime()))
              .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn.ToLocalTime()))
              .ReverseMap();
        }
    }
}

﻿namespace Models.Todo
{
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

        public virtual void Mapping(Profile mapper)
        {
            mapper.CreateMap<TodoItem, TodoItemDto>();
            mapper.CreateMap<TodoItemDto, TodoItem>();
        }
    }
}
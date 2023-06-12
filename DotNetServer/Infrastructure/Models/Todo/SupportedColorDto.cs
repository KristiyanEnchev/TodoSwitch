namespace Models.Todo
{
    using AutoMapper;

    using Domain.Entities;

    using Shared.Mappings;

    public class SupportedColorDto : IMapFrom<Color>
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public virtual void Mapping(Profile mapper)
        {
            mapper.CreateMap<SupportedColorDto, Color>().ReverseMap();
        }
    }
}
using AutoMapper;

namespace Template.Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //CreateMap<Domain.Task, TaskDto>()
            //    .ForMember(dest => dest.CategoryDto, opt => opt.MapFrom(src => src.Category))
            //    .ForMember(dest => dest.PriorityDto, opt => opt.MapFrom(src => src.Priority))
            //    .ReverseMap();
        }
    }
}

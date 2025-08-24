using AutoMapper;
using College.Data;
using College.Models;

namespace College.Configurations
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Student, StudentDto>().ReverseMap();
        }
    }
}
using AutoMapper;
using Entities.DTOs.Service;
using Entities.Model;

namespace Services.Mapping
{
    public class ServiceMappingProfile : Profile
    {
        public ServiceMappingProfile()
        {
            CreateMap<Service, ServiceDto>().ReverseMap();
        }
    }
}

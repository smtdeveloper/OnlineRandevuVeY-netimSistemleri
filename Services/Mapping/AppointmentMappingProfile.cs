using AutoMapper;
using Entities.DTOs.Appointment;
using Entities.Model;

namespace Services.Mapping;

public class AppointmentMappingProfile : Profile
{
    public AppointmentMappingProfile()
    {
        CreateMap<Appointment, AppointmentDto>().ReverseMap();
        CreateMap<AppointmentDto, UpdateAppointmentRequest>().ReverseMap();

    }
}

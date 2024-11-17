using AutoMapper;
using Entities.DTOs.Auth;
using Entities.Model;

namespace Services.Mapping
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles.Select(r => r.Role).ToList()))
                .ReverseMap();

            CreateMap<CreateUserRequest, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => HashPassword(src.Password)));

            CreateMap<User, UpdateUserRequest>().ReverseMap();
            CreateMap<UserDto, UpdateUserRequest>().ReverseMap();


        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
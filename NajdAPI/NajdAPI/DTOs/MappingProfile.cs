using AutoMapper;
using NajdAPI.DTOs.Users;
using NajdAPI.Models;

namespace NajdAPI.DTOs;

public class MappingProfile : Profile 
{
    public MappingProfile()
    {
        CreateMap<UserRegisterDTO, User>();
        CreateMap<User, UserProfileDTO>();
        CreateMap<UserAuthDTO, User>();
    }
}

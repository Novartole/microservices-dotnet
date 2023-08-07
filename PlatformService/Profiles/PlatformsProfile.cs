using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Profiles;

public class PlatformsProfile : Profile
{
    public PlatformsProfile()
    {
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<PlatformCreateDto, Platform>();
        CreateMap<PlatformReadDto, PlatformPublishedDto>();

        CreateMap<Platform, GrpcPlatfromModel>()
            .ForMember(
                dest => dest.PlatformId, 
                memOpts => memOpts.MapFrom(src => src.Id)
            ).ForMember(
                dest => dest.Name, 
                memOpts => memOpts.MapFrom(src => src.Name)
            ).ForMember(
                dest => dest.Publisher, 
                memOpts => memOpts.MapFrom(src => src.Publisher)
            );
    }   
}
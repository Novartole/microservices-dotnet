using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;
using PlatformService;

namespace CommandsService.Profiles;

public class CommandsProfile : Profile
{
    public CommandsProfile()
    {
        CreateMap<Command, CommandReadDto>();
        CreateMap<CommandCreateDto, Command>();

        CreateMap<Platform, PlatformReadDto>();

        CreateMap<PlatformPublishedDto, Platform>()
            .ForMember(
                dest => dest.ExternalId, 
                opt => opt.MapFrom(src => src.Id)
            );

        CreateMap<GrpcPlatfromModel, Platform>()
            .ForMember(
                dest => dest.ExternalId, 
                memOpts => memOpts.MapFrom(src => src.PlatformId)
            // not mandatory
            ).ForMember(
                dest => dest.Name, 
                memOpts => memOpts.MapFrom(src => src.Name)
            ).ForMember(
                dest => dest.Commands,
                memOpts => memOpts.Ignore()
            );
    }
}
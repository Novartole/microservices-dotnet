using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandsService.SyncDataServices.Grpc;

public class PlatformDataClient : IPlatformDataClient
{
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public PlatformDataClient(IConfiguration configuration, IMapper mapper)
    {
        _configuration = configuration;
        _mapper = mapper;
    }

    public IEnumerable<Platform> ReturnAllPlatforms()
    {
        System.Console.WriteLine($"--> Calling Grpc service at {_configuration.GetValue<string>("GrpcPlatform")}");

        var channel = GrpcChannel.ForAddress(_configuration.GetValue<string>("GrpcPlatform"));
        var client = new GrpcPlatform.GrpcPlatformClient(channel);
        var request = new GetAllRequest();
        try
        {
            var reply = client.GetAllPlatforms(request);
            return _mapper.Map<IEnumerable<Platform>>(reply.Platforms);
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine($"--> Could not call Grpc service {ex.Message}");

            return null;
        }
    }
}
using AutoMapper;
using Grpc.Core;
using PlatformService.Data;

namespace PlatformService.SyncServices.Grpc;

public class GrpcPlatfromService : GrpcPlatform.GrpcPlatformBase
{
    private readonly IPlatformRepo _repository;
    private readonly IMapper _mapper;

    public GrpcPlatfromService(IPlatformRepo repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public override Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
    {
        var response = new PlatformResponse();
        
        var platforms = _repository.GetAllPlatforms();
        foreach (var platform in platforms)
        {
            response.Platforms.Add(_mapper.Map<GrpcPlatfromModel>(platform));
        }

        return Task.FromResult(response);
    }
}
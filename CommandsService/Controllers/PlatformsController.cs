using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Controller]
[Route("api/c/[controller]")]
public class PlatformsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ICommandRepo _repository;

    public PlatformsController(ICommandRepo repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms() {
        Console.WriteLine("--> Getting Platforms from CommandService.");

        var platformsItem = _repository.GetAllPlatforms();

        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformsItem));
    }

    [HttpPost]
    public ActionResult TestInboundConnection() {
        Console.WriteLine("--> Inbound POST # CommandsServcie");

        return Ok("Inbound test of Platform controller");
    }
}
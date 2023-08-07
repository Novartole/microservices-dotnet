using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[ApiController]
[Route("/api/c/platforms/{platformId}/[controller]")]
public class CommandsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ICommandRepo _repository;

    public CommandsController(ICommandRepo repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId) {
        System.Console.WriteLine("--> Hit GetCommandsForPlatform", platformId);

        if (!_repository.DoesPlatfromExist(platformId)) {
            return NotFound();
        }

        var commandsItem = _repository.GetCommandsForPlatform(platformId);

        return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commandsItem));
    }

    [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
    public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId) {
        System.Console.WriteLine($"--> Hit GetCommandForPlatform {platformId} / {commandId}");

        if (!_repository.DoesPlatfromExist(platformId)) {
            return NotFound();
        }

        var commandItem = _repository.GetCommand(platformId, commandId);

        if (commandItem == null) {
            return NotFound();
        }

        return Ok(_mapper.Map<CommandReadDto>(commandItem));
    }

    [HttpPost]
    public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto command) {
        System.Console.WriteLine($"--> Hit CreateCommandForPlatform {platformId}");

        if (!_repository.DoesPlatfromExist(platformId)) {
            return NotFound();
        }

        var commandItem = _mapper.Map<Command>(command);

        _repository.CreateCommand(platformId, commandItem);
        _repository.SaveChanges();

        var commandReadDto = _mapper.Map<CommandReadDto>(commandItem);

        return CreatedAtRoute(
            nameof(GetCommandForPlatform),
            new { commandId = commandReadDto.Id, platformId = platformId },
            commandReadDto
        );
    }
}
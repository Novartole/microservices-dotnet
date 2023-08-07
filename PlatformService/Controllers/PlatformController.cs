using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncServices.Http;

namespace PlatformService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformRepo _repository;
    private readonly IMapper _mapper;
    private readonly ICommandDataClient _commandDataClient;
    private readonly IMessageBusClient _messageBusClient;

    public PlatformsController(
        IPlatformRepo repository, 
        IMapper mapper, 
        ICommandDataClient commandClient,
        IMessageBusClient messageBusClient
    ) {
        _repository = repository;
        _mapper = mapper;
        _commandDataClient = commandClient;
        _messageBusClient = messageBusClient;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms() {
        Console.WriteLine("--> Getting all platforms.");

        var platformsItem = _repository.GetAllPlatforms();

        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformsItem));
    }

    [HttpGet("{id}", Name = "GetPlatformById")]
    public ActionResult<PlatformReadDto> GetPlatformById(int id) {
        Console.WriteLine("--> Getting a platform by Id.");

        var platformItem = _repository.GetPlatformById(id);
        if (platformItem != null) {
            return Ok(_mapper.Map<PlatformReadDto>(platformItem));
        }

        return NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platform) {
        var platformModel = _mapper.Map<Platform>(platform);
        _repository.CreatePlatform(platformModel);
        _repository.SaveChanges();

        var platformReadDtoItem = _mapper.Map<PlatformReadDto>(platformModel);

        // Send sync message
        try
        {
            await _commandDataClient.SendPlatformCommand(platformReadDtoItem);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine("--> Cound not send syncronously: ", ex.Message);
        }

        // Send async message
        try
        {
            var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDtoItem);
            platformPublishedDto.Event = "Platform_Published";

            _messageBusClient.PublishNewPlatform(platformPublishedDto);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine("--> Cound not send asyncronously: ", ex.Message);
        }

        return CreatedAtRoute(
            nameof(GetPlatformById), 
            new { Id =  platformReadDtoItem.Id }, 
            platformReadDtoItem
        );
    }
}
using AutoMapper;
using System.Text.Json;
using CommandsService.Dtos;
using CommandsService.Models;
using CommandsService.Data;

namespace CommandsService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    enum EventType {
        Undetermined,
        PlatformPublished
    }

    private readonly IMapper _mapper;

    private readonly IServiceScopeFactory _scopeFactory;

    public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
    }

    private EventType DetermineEvent(string notificationMessage) {
        System.Console.WriteLine("--> Determining event...");

        var inputEvent = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

        switch (inputEvent.Event)
        {
            case "Platform_Published":
                System.Console.WriteLine("--> Platform_Published event detected");
                return EventType.PlatformPublished;

            default:
                System.Console.WriteLine("--> Could not determine event type");
                return EventType.Undetermined;
        }
    }

    public void ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);

        switch (eventType)
        {
            case EventType.PlatformPublished:
                AddPlatform(message);
                break;

            default:
                break;
        }
    }

    private void AddPlatform(string platformPublishedMessage) {
        using var scopeFactory = _scopeFactory.CreateScope();

        var repository = scopeFactory.ServiceProvider.GetRequiredService<ICommandRepo>();

        var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);
        try
        {
            var platform = _mapper.Map<Platform>(platformPublishedDto);

            if (repository.DoesExternalPlatformExist(platform.ExternalId)) {
                System.Console.WriteLine($"--> Platform {platform.ExternalId} does exist");
                return;
            }

            repository.CreatePlatform(platform);
            repository.SaveChanges();

            System.Console.WriteLine("--> New Platform added!");
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine($"--> Could not add platform to DB: {ex.Message}");
        }
    }
}
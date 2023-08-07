using CommandsService.Models;

namespace CommandsService.Data;

public class CommandRepo : ICommandRepo
{
    private readonly AppDbContext _context;

    public CommandRepo(AppDbContext context)
    {
        _context = context;
    }

    public bool DoesExternalPlatformExist(int externalPlatformId) {
        return _context.Platforms.Any(p => p.ExternalId == externalPlatformId);
    }

    public void CreateCommand(int platformId, Command commmand)
    {
        if (commmand == null) {
            throw new ArgumentNullException(nameof(commmand));
        }

        commmand.PlatformId = platformId;

        _context.Commands.Add(commmand);
    }

    public void CreatePlatform(Platform plat)
    {
        if (plat == null) {
            throw new ArgumentNullException(nameof(plat));
        }

        _context.Platforms.Add(plat);
    }

    public bool DoesPlatfromExist(int platformId)
    {
        return _context.Platforms.Any(p => p.Id == platformId);
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return _context.Platforms.ToList();
    }

    public Command GetCommand(int platformId, int commandId)
    {
        return _context.Commands
            .FirstOrDefault(c => c.PlatformId == platformId && c.Id == commandId);
    }

    public IEnumerable<Command> GetCommandsForPlatform(int platformId)
    {
        return _context.Commands
            .Where(c => c.PlatformId == platformId)
            .OrderBy(c => c.Platform.Name);
    }

    public bool SaveChanges()
    {
        return _context.SaveChanges() >= 0;
    }
}
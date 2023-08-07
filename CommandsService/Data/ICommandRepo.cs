using CommandsService.Models;

namespace CommandsService.Data;

public interface ICommandRepo
{
    bool SaveChanges();

    // Platfroms
    IEnumerable<Platform> GetAllPlatforms();
    void CreatePlatform(Platform plat);
    bool DoesPlatfromExist(int platformId);
    bool DoesExternalPlatformExist(int externalPlatformId);

    // Commands
    IEnumerable<Command> GetCommandsForPlatform(int platformId);
    Command GetCommand(int platformId, int commandId);
    void CreateCommand(int platformId, Command commmand);
}
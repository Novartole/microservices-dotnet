using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;

namespace CommandsService.Data;

public static class PrepDb {
    public static void PopulateData(IApplicationBuilder builder) {
        using var serviceScope = builder.ApplicationServices.CreateScope();
        var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
        
        var platforms = grpcClient.ReturnAllPlatforms();
    
        SeedData(serviceScope.ServiceProvider.GetService<ICommandRepo>(), platforms);    
        
    }

    private static void SeedData(ICommandRepo repo, IEnumerable<Platform> platforms)
    {
        System.Console.WriteLine("--> Seeding new Platforms...");

        foreach (var plat in platforms)
        {
            if (repo.DoesExternalPlatformExist(plat.ExternalId)) {
                continue;
            }

            repo.CreatePlatform(plat);
            repo.SaveChanges();
        }
    }
}
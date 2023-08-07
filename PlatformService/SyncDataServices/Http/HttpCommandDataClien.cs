using System.Text;
using System.Text.Json;
using PlatformService.Dtos;

namespace PlatformService.SyncServices.Http;

public class HttpCommandDataClien : ICommandDataClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public HttpCommandDataClien(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task SendPlatformCommand(PlatformReadDto plat)
    {
        var httpContext = new StringContent(
            JsonSerializer.Serialize(plat), 
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync(_config["CommandService"], httpContext);

        if (response.IsSuccessStatusCode) {
            Console.WriteLine("--> Sync POST to CommandService was OK");
        } else {
            Console.WriteLine("--> Sync POST to CommandService was NOT OK");
        }
    }
}
using System.Net;
using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncServices.Grpc;
using PlatformService.SyncServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
if (builder.Environment.IsDevelopment()) {

    Console.WriteLine("--> Use InMem db");

    builder.Services.AddDbContext<AppDbContext>(opt => 
        opt.UseInMemoryDatabase("InMem"));

    // builder.WebHost.ConfigureKestrel(opts => 
    //     opts.ListenLocalhost(
    //         5666, 
    //         o => o.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2
    //     )
    // );

    // builder.WebHost.ConfigureKestrel(opts => 
    //     opts.ListenLocalhost(
    //         5050, 
    //         o => o.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2
    //     )
    // );

} else if (builder.Environment.IsProduction()) {

    Console.WriteLine("--> Use PostgreSQL db");

    builder.Services.AddDbContext<AppDbContext>(opt => 
        opt.UseNpgsql(builder.Configuration.GetConnectionString("PlatformsConn")));

}

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// inject service to call another endpoint via http
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClien>();

// inject service to call another endpoint via grpc
builder.Services.AddGrpc();

// inject service to send messages into message bus (rbtmq)
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddHttpsRedirection(options =>
// {
//     options.RedirectStatusCode = (int)HttpStatusCode.TemporaryRedirect;
//     options.HttpsPort = 7098;
// });

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGrpcService<GrpcPlatfromService>();

app.MapGet("/protos/platforms.proto", async context => {
    await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
});

PrepDb.PrepPopulation(app, builder.Environment.IsProduction());

app.Run();


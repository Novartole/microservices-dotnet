using PlatformService.Dtos;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace PlatformService.AsyncDataServices;

public class MessageBusClient : IMessageBusClient
{
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageBusClient(IConfiguration configuration)
    {
        _configuration = configuration;

        var factory = new ConnectionFactory () {
            HostName = _configuration.GetValue<string>("RabbitMQHost"),
            Port = _configuration.GetValue<int>("RabbitMQPort")
        };
        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            /* 
                exchange: name of exchange
                type: ExchangeType.Fanout ( ~ broadcasts )
            */
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

            System.Console.WriteLine("--> Connected to Message bus");
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine($"--> Couldn't connect to RabbitMQ: {ex.Message}");
        }
    }

    public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
    {
        var message = JsonSerializer.Serialize(platformPublishedDto);

        if (_connection.IsOpen) {
            System.Console.WriteLine("--> Connection to Message bus is open, sending message...");

            SendMessage(message);
        } else {
            System.Console.WriteLine("--> RabbitMQ connection is closed, not sending");
        }
    }

    private void SendMessage(string message) {
        var body = Encoding.UTF8.GetBytes(message);

        /*
            exchange: "trigger" - named exchange is used
                                  but it's also possible to use default one by setting "" as the value
            routingKey: "" - path of a queue ("" ~ default one is used)
        */ 
        _channel.BasicPublish(
            exchange: "trigger",
            routingKey: "",
            basicProperties: null,
            body: body
        );

        System.Console.WriteLine($"--> We have sent message {message}");
    }

    private void Dispose() {
        System.Console.WriteLine("--> Message bus Disposed");

        if (_connection.IsOpen) {
            _channel.Close();
            _connection.Close();
        }
    }

    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) {
        System.Console.WriteLine("--> RabbitMQ Connection shutdown");
    }
}
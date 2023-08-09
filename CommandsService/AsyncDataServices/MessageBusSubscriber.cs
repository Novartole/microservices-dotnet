
using System.Text;
using CommandsService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataServices;

public class MessageBusSubscriber : BackgroundService
{
    private readonly IEventProcessor _eventProcessor;
    private readonly IConfiguration _configuration;

    private IConnection _connection;
    private IModel _channel;
    private string _queueName;

    public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
    {
        _configuration = configuration;
        _eventProcessor = eventProcessor;

        InitializeRabbitMQ();
    }

    private void InitializeRabbitMQ() {
        var factory = new ConnectionFactory() {
            HostName = _configuration.GetValue<string>("RabbitMQHost"),
            Port = _configuration.GetValue<int>("RabbitMQPort")
        };

        _connection = factory.CreateConnection();

        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(
            exchange: "trigger",
            type: ExchangeType.Fanout
        );

        // create a non-durable, exclusive, autodelete queue with a generated name
        _queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(
            queue: _queueName,
            exchange: "trigger",
            routingKey: ""
        );

        System.Console.WriteLine("--> Listening on Message bus...");

        _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
    }

    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) {
        System.Console.WriteLine("--> Connection to Message Shutdown");
    }

    public override void Dispose()
    {
        if (_channel.IsOpen) {
            _channel.Close();
            _connection.Close();
        }

        base.Dispose();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (sender, e) => {
            System.Console.WriteLine("--> Event received");

            var body = e.Body;
            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

            _eventProcessor.ProcessEvent(notificationMessage);
        };

        _channel.BasicConsume(
            queue: _queueName, 
            /* 
                send confirmation to Message bus automatically
                otherwise a manual command ( ack (+), nack ( -* ), reject (-) ) have to be sent
            */
            autoAck: true, 
            consumer: consumer
        );

        return Task.CompletedTask;
    }
}
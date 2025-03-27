using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class RabbitMqConsumerService : BackgroundService
{
    private readonly ConnectionFactory _factory;
    private readonly string[] _topics; // 👈 Guardamos los topics
    private IConnection? _connection;
    private IChannel? _channel;
    private string? _queueName;


    public RabbitMqConsumerService(IConfiguration configuration)
    {
        var rabbitMqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "rabbitmq";
        var rabbitMqUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "user"; // Default user
        var rabbitMqPass = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "password"; // Default password


        _factory = new ConnectionFactory()
        {
            HostName = rabbitMqHost,
            UserName = rabbitMqUser,
            Password = rabbitMqPass
        };
        _topics = configuration.GetSection("Topics").Get<string[]>() ?? new[] { "#" }; // 👈 Leemos los topics desde la configuración
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Initializing RabbitMQ connection...");

        _connection = await _factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        // Declaramos el exchange
        await _channel.ExchangeDeclareAsync(exchange: "topic_logs", type: ExchangeType.Topic);

        // Declaramos una cola temporal
        QueueDeclareOk queueDeclareResult = await _channel.QueueDeclareAsync();
        _queueName = queueDeclareResult.QueueName;

        // Vinculamos la cola con los topics proporcionados
        foreach (var topic in _topics)
        {
            await _channel.QueueBindAsync(queue: _queueName, exchange: "topic_logs", routingKey: topic);
            Console.WriteLine($" [*] Listening to topic: {topic}");
        }

        Console.WriteLine("RabbitMQ Consumer Started.");
        Console.WriteLine(" [*] Waiting for messages...");

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;
            Console.WriteLine($" [x] Received '{routingKey}':'{message}'");

            await Task.CompletedTask;
        };

        await _channel.BasicConsumeAsync(_queueName, autoAck: true, consumer: consumer);

        // Mantener el servicio en ejecución hasta que se detenga
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

}

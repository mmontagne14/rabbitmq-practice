using RabbitMQ.Client;
using System.Text;
using System.Net.Http;


var rabbitMqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "rabbitmq";
var rabbitMqUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "user"; // Default user
var rabbitMqPass = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "password"; // Default password


var factory = new ConnectionFactory()
{
    HostName = rabbitMqHost,
    UserName = rabbitMqUser,
    Password = rabbitMqPass
};


using var connection = await factory.CreateConnectionAsync();

using var channel = await connection.CreateChannelAsync();

// Creamos un Exchange de tipo Topic el cual enviara los mensaje correspondientes a las colas cuyo binding key contengan temas que coincidan con los routing key de los mensajes
await channel.ExchangeDeclareAsync(exchange: "topic_logs", type: ExchangeType.Topic);

var routingKey = (args.Length > 0) ? args[0] : "anonymous.info";
var message = (args.Length > 1) ? string.Join(" ", args.Skip(1).ToArray()) : "Hello World!";

var body = Encoding.UTF8.GetBytes(message);
await channel.BasicPublishAsync(exchange: "topic_logs", routingKey: routingKey, body: body);


Console.WriteLine($" [x] Sent '{routingKey}':'{message}'");

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();


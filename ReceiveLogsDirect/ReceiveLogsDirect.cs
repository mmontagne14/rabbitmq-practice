using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

if (args.Length < 1)
{
    Console.Error.WriteLine("Usage: {0} [info] [warning] [error]",
                            Environment.GetCommandLineArgs()[0]);
    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
    Environment.ExitCode = 1;
    return;
}


var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

// Creamos un Exchange de tipo Direct el cual enviará a las colas los mensajes cuyo binding key coincidan con su routing key
await channel.ExchangeDeclareAsync(exchange: "direct_logs", type: ExchangeType.Direct);


QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync();
string queueName = queueDeclareResult.QueueName;

//Creamos un binding por cada severidad en la que estemos interesados
foreach (string? severity in args)
{
    await channel.QueueBindAsync(queue: queueName, exchange: "direct_logs", routingKey: severity);
}


Console.WriteLine(" [*] Waiting for logs.");

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var routingKey = ea.RoutingKey;
    Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
    return Task.CompletedTask;
};

await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
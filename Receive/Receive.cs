using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

//declara la cola igual que el productor
await channel.QueueDeclareAsync(queue: "hello", durable: false, exclusive: false, autoDelete: false,
    arguments: null);

Console.WriteLine(" [*] Waiting for messages.");


var consumer = new AsyncEventingBasicConsumer(channel); // Crea un consumidor asíncrono

//Evento que se activa cuando RabbitMQ entrega un mensaje.
consumer.ReceivedAsync += (model, ea) =>
{
    var body = ea.Body.ToArray(); //  Extrae el cuerpo del mensaje.
    var message = Encoding.UTF8.GetString(body); //  Convierte el mensaje de bytes a string.
    Console.WriteLine($" [x] Received {message}");
    return Task.CompletedTask; //  Indica que la tarea asíncrona terminó.
};

await channel.BasicConsumeAsync("hello", autoAck: true, consumer: consumer); // Le decimos a RabbitMQ que nos envíe mensajes


Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
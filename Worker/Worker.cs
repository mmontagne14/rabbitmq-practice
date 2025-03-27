using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

//declara la cola igual que el productor
await channel.QueueDeclareAsync(queue: "task_queue", durable: true, exclusive: false, autoDelete: false,
    arguments: null);

await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

Console.WriteLine(" [*] Waiting for messages.");


var consumer = new AsyncEventingBasicConsumer(channel); // Crea un consumidor asíncrono

//Evento que se activa cuando RabbitMQ entrega un mensaje.
consumer.ReceivedAsync += async (model, ea) =>
{
    var body = ea.Body.ToArray(); //  Extrae el cuerpo del mensaje.
    var message = Encoding.UTF8.GetString(body); //  Convierte el mensaje de bytes a string.
    Console.WriteLine($" [x] Received {message}");

    int dots = message.Split('.').Length - 1;
    await Task.Delay(dots * 1000);

    Console.WriteLine(" [x] Done");

    // aqui el canal también puede ser accedido como ((AsyncEventingBasicConsumer)sender).Channel
    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
};

await channel.BasicConsumeAsync("task_queue", autoAck: false, consumer: consumer); // Le decimos a RabbitMQ que nos envíe mensajes


Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
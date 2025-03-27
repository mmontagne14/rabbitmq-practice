using RabbitMQ.Client;
using System.Text;


// se conecta a un nodo RabbitMQ en la máquina local (podemos apuntar a otra máquina cambiando por su hostname o dirección ip)
var factory = new ConnectionFactory { HostName = "localhost" };

// abstrae la conexión y se encarga de los protocolos de negociación y de la autenticación por nosotros
using var connection = await factory.CreateConnectionAsync();

// creamos un canal, donde se encuentra la API para realizar el trabajo
using var channel = await connection.CreateChannelAsync();

// Para enviar declaramos la cola a la que enviaremos el mensaje, luego podemos publicarle el mensaje
// La creación de la cola es idempotent, se creará sólo si no existe.
await channel.QueueDeclareAsync(queue: "hello", durable: false, exclusive: false, autoDelete: false,
    arguments: null);


const string message = "Hello baby!";
//  El contenido del mensaje es un array de bytes, se puede codificar lo que queramos allí
var body = Encoding.UTF8.GetBytes(message);


await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "hello", body: body);
Console.WriteLine($" [x] Sent {message}");

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
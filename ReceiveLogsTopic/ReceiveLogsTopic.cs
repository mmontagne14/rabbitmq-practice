// using RabbitMQ.Client;
// using RabbitMQ.Client.Events;
// using System.Text;

// if (args.Length < 1)
// {
//     Console.Error.WriteLine("Usage: {0} [info] [warning] [error]",
//                             Environment.GetCommandLineArgs()[0]);
//     Console.WriteLine(" Press [enter] to exit.");
//     Console.ReadLine();
//     Environment.ExitCode = 1;
//     return;
// }

// var rabbitMqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
// var rabbitMqUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "user"; // Default user
// var rabbitMqPass = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "password"; // Default password

// var factory = new ConnectionFactory()
// {
//     HostName = rabbitMqHost,
//     UserName = rabbitMqUser,
//     Password = rabbitMqPass
// };
// using var connection = await factory.CreateConnectionAsync();
// using var channel = await connection.CreateChannelAsync();

// // Creamos un Exchange de tipo Topic el cual enviara los mensaje correspondientes a las colas cuyo binding key contengan temas que coincidan con los routing key de los mensajes
// await channel.ExchangeDeclareAsync(exchange: "topic_logs", type: ExchangeType.Topic);


// QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync();
// string queueName = queueDeclareResult.QueueName;

// //Creamos un binding por cada topic en el que estemos interesados
// foreach (string? bindingKey in args)
// {
//     await channel.QueueBindAsync(queue: queueName, exchange: "topic_logs", routingKey: bindingKey);
// }


// Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");

// var consumer = new AsyncEventingBasicConsumer(channel);
// consumer.ReceivedAsync += (model, ea) =>
// {
//     var body = ea.Body.ToArray();
//     var message = Encoding.UTF8.GetString(body);
//     var routingKey = ea.RoutingKey;
//     Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
//     return Task.CompletedTask;
// };

// await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);

// Console.WriteLine(" Press [enter] to exit.");
// Console.ReadLine();
using RabbitMQ.Client;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

// Obtener configuración de RabbitMQ desde variables de entorno
var rabbitMqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "rabbitmq";
var rabbitMqUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "user";
var rabbitMqPass = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "password";

// Crear la conexión a RabbitMQ
var factory = new ConnectionFactory()
{
    HostName = rabbitMqHost,
    UserName = rabbitMqUser,
    Password = rabbitMqPass
};

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

// Crear un Exchange de tipo Topic
await channel.ExchangeDeclareAsync(exchange: "topic_logs", type: ExchangeType.Topic);

// Obtener imagen aleatoria de gato desde la API
var catImageUrl = await GetRandomCatImage();
var message = $"Mira este gato: {catImageUrl}";

// Definir el routing key (tema)
var routingKey = "info.cats";
var body = Encoding.UTF8.GetBytes(message);

// Publicar el mensaje en RabbitMQ
await channel.BasicPublishAsync(exchange: "topic_logs", routingKey: routingKey, body: body);

Console.WriteLine($" [x] Sent '{routingKey}':'{message}'");
Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

// Método para obtener una imagen aleatoria de TheCatAPI
async Task<string> GetRandomCatImage()
{
    using var httpClient = new HttpClient();
    string url = "https://api.thecatapi.com/v1/images/search";

    try
    {
        var response = await httpClient.GetStringAsync(url);
        var images = JsonSerializer.Deserialize<CatImage[]>(response);
        return images?[0]?.Url ?? "No se pudo obtener la imagen";
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error obteniendo imagen: {ex.Message}");
        return "Error obteniendo imagen";
    }
}

// Modelo para deserializar la respuesta JSON de TheCatAPI
class CatImage
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }
}

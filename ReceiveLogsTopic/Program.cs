using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;

class Program
{
    public static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                var topics = args.Length > 0 ? args : new[] { "#" }; // 👈 Si no hay argumentos, escucha todo
                                                                     // ✅ Convertimos correctamente a KeyValuePair<string, string?>
                var inMemorySettings = new Dictionary<string, string?>
                {
                    { "Topics", string.Join(",", topics) }
                };

                config.AddInMemoryCollection(inMemorySettings);
            })
             .ConfigureServices((hostContext, services) =>
             {
                 services.AddHostedService<RabbitMqConsumerService>();
             })
             .Build();

        host.Run();
    }

}

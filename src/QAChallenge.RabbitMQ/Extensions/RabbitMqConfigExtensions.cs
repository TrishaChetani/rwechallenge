using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QAChallenge.RabbitMQ.Models;

namespace QAChallenge.RabbitMQ.Extensions;

public static class RabbitMqConfigExtensions
{
    public static IDictionary<string, ConnectionSpec> RabbitMqConnectionSpecs(
        this IConfiguration config,
        string sectionName = "RabbitMQ:Connections") => config
        .GetSection(sectionName)
        .Get<IDictionary<string, ConnectionSpec>>();

    public static IServiceCollection ConfigureRabbitMqConnectionConfigs(this IServiceCollection services,
        IConfiguration config, string sectionName = "RabbitMQ:Connections")
    {
        var section = config.GetSection(sectionName);
        var specs = section.Get<IDictionary<string, ConnectionSpec>>();

        foreach (var (key, value) in specs)
        {
            services.Configure<ConnectionSpec>(key, section.GetSection(key));
        }

        return services;
    }
}

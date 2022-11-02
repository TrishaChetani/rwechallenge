using System.Net;
using RabbitMQ.Client;

namespace QAChallenge.RabbitMQ.Models;

public record ConnectionSpec
{
    public string HostName { get; init; } = string.Empty;
    public ushort Port { get; init; } = 5672;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string VHost { get; init; } = "/";
    public uint PrefetchSize { get; init; }
    public ushort PrefetchCount { get; init; } = 20;

    public virtual IConnectionFactory ToFactory() => new ConnectionFactory()
    {
        HostName = HostName,
        UserName = Username,
        Password = Password,
        VirtualHost = VHost,
        Port = Port,
        DispatchConsumersAsync = true,
        ClientProvidedName = Dns.GetHostName(),
        AutomaticRecoveryEnabled = true,
        TopologyRecoveryEnabled = true
    };

    public virtual string ConnectionString()
    {
        return new UriBuilder("amqp", HostName, Port, VHost)
        {
            UserName = Username,
            Password = Password,
        }.ToString();
    }
}

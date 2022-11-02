using RabbitMQ.Client;

namespace QAChallenge.RabbitMQ.Models;

public record PublishingAddress()
{
    public PublishingAddress(string exchange, string routingKey, bool mandatory) : this()
    {
        Exchange = exchange;
        RoutingKey = routingKey;
        Mandatory = mandatory;
    }

    public string Exchange { get; init; } = "amq.topic";
    public string Type { get; init; } = ExchangeType.Topic;
    public string RoutingKey { get; init; }
    public bool Mandatory { get; init; }
}

public record PublisherRegistrationSpec
{
    public PublishingAddress Address { get; init; }
    public string ConnectionReference { get; init; }
}

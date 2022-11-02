using RabbitMQ.Client;

namespace QAChallenge.RabbitMQ.Models;

public record ExchangeSpec()
{
    public ExchangeSpec(string name, string type) : this()
    {
        Name = name;
        Type = type;
    }

    public string Name { get; init; }
    public string Type { get; init; } = ExchangeType.Topic;
}

public record QueueSpec()
{
    public QueueSpec(string name, bool durable, bool exclusive, bool autoDelete, Dictionary<string, object> arguments) : this()
    {
        Name = name;
        Durable = durable;
        Exclusive = exclusive;
        AutoDelete = autoDelete;
        Arguments = arguments;
    }

    public string Name { get; init; }
    public bool Durable { get; init; }
    public bool Exclusive { get; init; }
    public bool AutoDelete { get; init; }
    public Dictionary<string, object>? Arguments { get; init; }

    internal Dictionary<string, object> ParseArgs()
    {
        var parsed = new Dictionary<string, object>();
        if (Arguments is null)
        {
            return parsed;
        }
        foreach (var (key, value) in Arguments)
        {
            if (value is string s && long.TryParse(s, out var numberVal))
            {
                parsed[key] = numberVal;
            }
            else
            {
                parsed[key] = value;
            }
        }

        return parsed;
    }
}

public record QueueBindingSpec()
{
    public QueueBindingSpec(string routingKey, Dictionary<string, object> arguments) : this()
    {
        RoutingKey = routingKey;
        Arguments = arguments;
    }

    public string RoutingKey { get; init; }
    public Dictionary<string, object> Arguments { get; init; }
}

public record ConsumerRegistrationSpec()
{
    public ConsumerRegistrationSpec(ExchangeSpec exchange, QueueSpec queue, QueueBindingSpec binding, string connectionReference) : this()
    {
        Exchange = exchange;
        Queue = queue;
        Binding = binding;
        ConnectionReference = connectionReference;
    }

    public ExchangeSpec Exchange { get; init; }
    public QueueSpec Queue { get; init; }
    public QueueBindingSpec? Binding { get; init; }
    public string ConnectionReference { get; init; }
}

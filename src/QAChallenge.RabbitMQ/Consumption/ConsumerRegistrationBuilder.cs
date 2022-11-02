using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using QAChallenge.RabbitMQ.Encoding;
using QAChallenge.RabbitMQ.Models;
using RabbitMQ.Client;

namespace QAChallenge.RabbitMQ.Consumption;

public record ConsumerRegistrationBuilder<TMessage> where TMessage : notnull
{
    private readonly IServiceCollection _services;
    private ExchangeSpec? _exchange;
    private QueueSpec? _queue;
    private QueueBindingSpec? _binding;
    private string? _connectionReference;

    public ConsumerRegistrationBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public ConsumerRegistrationBuilder<TMessage> WithRegistrationSpec(ConsumerRegistrationSpec spec) => this with
    {
        _queue = spec.Queue,
        _binding = spec.Binding,
        _exchange = spec.Exchange,
        _connectionReference = spec.ConnectionReference
    };

    public ConsumerRegistrationBuilder<TMessage> WithExchange(ExchangeSpec spec) => this with { _exchange = spec };

    public ConsumerRegistrationBuilder<TMessage> WithExchange(string name, string type = ExchangeType.Topic) =>
        this with { _exchange = new ExchangeSpec(name, type) };

    public ConsumerRegistrationBuilder<TMessage> WithQueue(QueueSpec spec) => this with { _queue = spec };

    public ConsumerRegistrationBuilder<TMessage> WithQueue(
        string name,
        bool durable = true,
        bool exclusive = false,
        bool autoDelete = false,
        Dictionary<string, object> args = null
    )
        => this with { _queue = new QueueSpec(name, durable, exclusive, autoDelete, args) };

    public ConsumerRegistrationBuilder<TMessage> WithBinding(QueueBindingSpec spec) => this with { _binding = spec };

    public ConsumerRegistrationBuilder<TMessage> WithBinding(
        string routingKey,
        Dictionary<string, object> args = null
    ) => this with { _binding = new QueueBindingSpec(routingKey, args) };

    public ConsumerRegistrationBuilder<TMessage> WithConnectionReference(string reference) =>
        this with { _connectionReference = reference };

    public IServiceCollection RegisterJsonConsumer<TConsumer>(JsonSerializerOptions? serializerOptions = null, JsonTypeInfo<TMessage>? messageTypeInfo = null) where TConsumer : class, IMessageConsumer<TMessage>
    {
        _services.TryAddScoped<IMessageConsumer<TMessage>, TConsumer>();
        _services.AddTransient<IMessageConsumerRegistration>(sp => new AsyncDefaultConsumer<TMessage>(
            sp.GetRequiredService<ILogger<AsyncDefaultConsumer<TMessage>>>(),
            new ConsumerRegistrationSpec(_exchange, _queue, _binding, _connectionReference),
            new Json<TMessage>(serializerOptions, messageTypeInfo),
            sp
        ));

        return _services;
    }
}

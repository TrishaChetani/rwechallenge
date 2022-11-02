using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using QAChallenge.RabbitMQ.Encoding;
using QAChallenge.RabbitMQ.Models;

namespace QAChallenge.RabbitMQ.Publishing;

public record PublisherRegistrationBuilder<TMessage> where TMessage : notnull
{
    private readonly IServiceCollection _services;

    private PublishingAddress? _publisher;
    private string? _connectionReference;

    public PublisherRegistrationBuilder(IServiceCollection services)
    {
        _services = services;
        _publisher = null;
    }

    public PublisherRegistrationBuilder<TMessage> WithConnectionReference(string reference) =>
        this with { _connectionReference = reference };

    public PublisherRegistrationBuilder<TMessage> WithPublishingAddress(PublishingAddress spec) =>
        this with { _publisher = spec };

    public PublisherRegistrationBuilder<TMessage> WithPublishingAddress(string exchange, string routingKey,
        bool mandatory = false) =>
        this with { _publisher = new PublishingAddress(exchange, routingKey, mandatory) };

    public PublisherRegistrationBuilder<TMessage> WithRegistrationSpec(PublisherRegistrationSpec spec) => this
        with
    {
        _publisher = spec.Address,
        _connectionReference = spec.ConnectionReference
    };

    public IServiceCollection RegisterJsonPublisher(JsonSerializerOptions? serializerOptions = null, JsonTypeInfo<TMessage> messageTypeInfo = null)
    {
        _services.TryAddScoped<IMessagePublisher<TMessage>>(sp => new MessagePublisher<TMessage>(
            sp.GetRequiredService<IChannelFactory>().GetChannel(_connectionReference),
            new Json<TMessage>(serializerOptions, messageTypeInfo),
            _publisher
        ));

        return _services;
    }
}

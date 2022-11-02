using System.Diagnostics;
using QAChallenge.RabbitMQ.Models;
using RabbitMQ.Client;

namespace QAChallenge.RabbitMQ.Publishing;

public sealed class MessagePublisher<TMessage> : IMessagePublisher<TMessage>, IDisposable where TMessage : notnull
{
    private readonly IModel _channel;
    private readonly IMessageEncoder<TMessage> _encoder;
    private readonly PublishingAddress _publishingAddress;

    public MessagePublisher(IModel channel, IMessageEncoder<TMessage> encoder, PublishingAddress publishingAddress)
    {
        _channel = channel;
        _encoder = encoder;
        _publishingAddress = publishingAddress;
    }

    public void Publish(TMessage message)
    {
        _channel.ExchangeDeclare(_publishingAddress.Exchange, _publishingAddress.Type, true);

        var payload = _encoder.Encode(message);

        var properties = _channel.CreateBasicProperties();
        properties.ContentType = _encoder.ContentType;
        properties.MessageId = Guid.NewGuid().ToString();
        properties.Type = typeof(TMessage).FullName;
        properties.CorrelationId = Activity.Current?.Id ?? string.Empty;

        _channel.BasicPublish(
            _publishingAddress.Exchange,
            _publishingAddress.RoutingKey,
            _publishingAddress.Mandatory,
            properties,
            payload
        );
    }

    public bool CanPublish() => _channel.IsOpen;

    public void Dispose()
    {
        _channel.Dispose();
    }
}

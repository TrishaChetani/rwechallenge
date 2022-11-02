using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QAChallenge.RabbitMQ.Models;
using RabbitMQ.Client;

namespace QAChallenge.RabbitMQ.Consumption;

public sealed class AsyncDefaultConsumer<TMessage> : AsyncDefaultBasicConsumer, IMessageConsumerRegistration where TMessage : notnull
{
    private readonly ILogger<AsyncDefaultConsumer<TMessage>> _logger;
    private readonly IMessageDecoder<TMessage> _decoder;
    private readonly IServiceProvider _serviceProvider;

    public AsyncDefaultConsumer(ILogger<AsyncDefaultConsumer<TMessage>> logger, ConsumerRegistrationSpec spec, IMessageDecoder<TMessage> decoder, IServiceProvider serviceProvider)
    : base(serviceProvider.GetRequiredService<IChannelFactory>().GetChannel(spec.ConnectionReference))
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _decoder = decoder;
        Spec = spec;
    }

    public override async Task HandleBasicDeliver(
        string consumerTag,
        ulong deliveryTag,
        bool redelivered,
        string exchange,
        string routingKey,
        IBasicProperties properties,
        ReadOnlyMemory<byte> body)
    {
        using var cancellationTokenSource = new CancellationTokenSource();

        try
        {
            var deserializedBody = _decoder.Deserialize(body);
            if (deserializedBody is null)
            {
                _logger.LogWarning("Deserialized message body is empty");
                return;
            }

            await using var scope = _serviceProvider.CreateAsyncScope();
            var consumer = scope.ServiceProvider.GetRequiredService<IMessageConsumer<TMessage>>();

            var result = await consumer.HandleAsync(
                new Message<TMessage>(deserializedBody),
                cancellationTokenSource.Token
            );

            switch (result.Acknowledge)
            {
                case AcknowledgeMode.Ack:
                    Model.BasicAck(deliveryTag, false);
                    break;
                case AcknowledgeMode.Nack:
                    Model.BasicNack(deliveryTag, false, result.Requeue);
                    break;
                case AcknowledgeMode.Reject:
                    Model.BasicReject(deliveryTag, result.Requeue);
                    break;
                default:
                    _logger.LogWarning("Unexpected acknowledge mode {Mode}", result.Acknowledge);
                    break;
            }
        }
        catch (JsonException e)
        {
            _logger.LogCritical(e, "Failed to unmarshal message");
            Model.BasicReject(deliveryTag, false);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Failed to handle incoming message");
            Model.BasicNack(deliveryTag, false, true);
        }
    }

    public ConsumerRegistrationSpec Spec { get; }
}

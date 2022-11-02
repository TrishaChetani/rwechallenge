using QAChallenge.RabbitMQ.Models;
using RabbitMQ.Client;

namespace QAChallenge.RabbitMQ;

public interface IMessageConsumerRegistration : IBasicConsumer
{
    ConsumerRegistrationSpec Spec { get; }
}

public enum AcknowledgeMode
{
    Ack,
    Nack,
    Reject
}

public record MessageResult(AcknowledgeMode Acknowledge, bool Requeue = false)
{
    public static MessageResult Ack() => new MessageResult(AcknowledgeMode.Ack);
    public static MessageResult Nack(bool requeue) => new MessageResult(AcknowledgeMode.Nack, requeue);
    public static MessageResult Reject(bool requeue = false) => new MessageResult(AcknowledgeMode.Reject, requeue);
}

public record Message<TBody>(TBody Body);

public interface IMessageConsumer<TMessage>
{
    Task<MessageResult> HandleAsync(Message<TMessage> message, CancellationToken token);
}

namespace QAChallenge.RabbitMQ;

public interface IMessagePublisher<TMessage> where TMessage : notnull
{
    void Publish(TMessage message);
    bool CanPublish();
}

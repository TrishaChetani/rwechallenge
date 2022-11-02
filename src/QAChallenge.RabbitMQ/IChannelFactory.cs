using RabbitMQ.Client;

namespace QAChallenge.RabbitMQ;

public interface IChannelFactory : IDisposable
{
    IModel GetChannel(string connectionReference);
}

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace QAChallenge.RabbitMQ;

public class RabbitMqConsumerHost : IHostedService
{
    private readonly ILogger<RabbitMqConsumerHost> _logger;
    private readonly IChannelFactory _channelFactory;
    private readonly IEnumerable<IMessageConsumerRegistration> _consumers;

    public RabbitMqConsumerHost(
        ILogger<RabbitMqConsumerHost> logger,
        IChannelFactory channelFactory,
        IEnumerable<IMessageConsumerRegistration> consumers)
    {
        _logger = logger;
        _channelFactory = channelFactory;
        _consumers = consumers;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var messageConsumer in _consumers)
        {
            RegisterConsumer(messageConsumer);
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _channelFactory.Dispose();
        return Task.CompletedTask;
    }

    private void RegisterConsumer(IMessageConsumerRegistration consumerRegistration)
    {
        try
        {
            var channel = consumerRegistration.Model;
            var exchangeSpec = consumerRegistration.Spec.Exchange;
            var queueSpec = consumerRegistration.Spec.Queue;

            channel.ExchangeDeclare(exchangeSpec.Name, exchangeSpec.Type, true, false, null);

            channel.QueueDeclare(
                queueSpec.Name,
                queueSpec.Durable,
                queueSpec.Exclusive,
                queueSpec.AutoDelete,
                queueSpec.ParseArgs()
            );

            var bindingSpec = consumerRegistration.Spec.Binding;
            if (bindingSpec is not null)
            {
                channel.QueueBind(queueSpec.Name, exchangeSpec.Name, bindingSpec.RoutingKey);
            }

            channel.BasicConsume(queueSpec.Name, false, consumerRegistration);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Failed to register consumer");
            throw;
        }
    }
}

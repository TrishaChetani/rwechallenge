using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using QAChallenge.RabbitMQ.Connection;
using QAChallenge.RabbitMQ.Consumption;
using QAChallenge.RabbitMQ.Publishing;

namespace QAChallenge.RabbitMQ.Extensions;

public static class RabbitMqRegistrationExtensions
{
    public static ConsumerRegistrationBuilder<TMessage> AddMessageConsumer<TMessage>(this IServiceCollection services) where TMessage : notnull
    {
        services.AddHostedService<RabbitMqConsumerHost>();
        services.TryAddSingleton<IChannelFactory, ChannelFactory>();
        return new ConsumerRegistrationBuilder<TMessage>(services);
    }

    public static PublisherRegistrationBuilder<TMessage> AddMessagePublisher<TMessage>(this IServiceCollection services)
        where TMessage : notnull
    {
        services.TryAddSingleton<IChannelFactory, ChannelFactory>();
        return new PublisherRegistrationBuilder<TMessage>(services);
    }
}

using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using QAChallenge.RabbitMQ.Models;
using RabbitMQ.Client;

namespace QAChallenge.RabbitMQ.Connection;

internal sealed class ChannelFactory : IChannelFactory, IDisposable
{
    private readonly object _lock;
    private readonly IOptionsMonitor<ConnectionSpec> _connectionOptions;
    private readonly IDictionary<string, IConnection> _connectionsByReference;
    private readonly IDictionary<string, ConcurrentQueue<IModel>> _channelsByConnection;

    public ChannelFactory(IOptionsMonitor<ConnectionSpec> connectionOptions)
    {
        _lock = new object();
        _connectionOptions = connectionOptions;
        _connectionsByReference = new Dictionary<string, IConnection>();
        _channelsByConnection = new Dictionary<string, ConcurrentQueue<IModel>>();
    }


    public IModel GetChannel(string connectionReference)
    {
        var spec = _connectionOptions.Get(connectionReference);
        if (spec is null)
        {
            throw new NoConnectionSpecException(connectionReference);
        }

        lock (_lock)
        {
            if (_channelsByConnection.TryGetValue(connectionReference, out var channelQueue))
            {
                return channelQueue.TryDequeue(out var channel)
                    ? new PooledChannel(channel, connectionReference, this)
                    : new PooledChannel(
                        CreateChannel(_connectionsByReference[connectionReference], spec),
                        connectionReference,
                        this
                    );
            }

            // if there's no queue, no channel was ever created
            var connection = spec.ToFactory().CreateConnection();
            _connectionsByReference[connectionReference] = connection;
            _channelsByConnection.Add(connectionReference, new ConcurrentQueue<IModel>());

            return new PooledChannel(connection.CreateModel(), connectionReference, this);
        }
    }

    internal void ReturnToPool(IModel channel, string connectionReference)
    {
        if (!channel.IsOpen)
        {
            return;
        }

        if (!_channelsByConnection.ContainsKey(connectionReference))
        {
            _channelsByConnection.Add(connectionReference, new ConcurrentQueue<IModel>());
        }

        _channelsByConnection[connectionReference].Enqueue(channel);
    }

    public void Dispose()
    {
        lock (_lock)
        {
            foreach (var (_, channelQueue) in _channelsByConnection)
            {
                while (!channelQueue.IsEmpty)
                {
                    if (channelQueue.TryDequeue(out var channel))
                    {
                        channel.Dispose();
                    }
                }
            }

            _channelsByConnection.Clear();

            foreach (var (_, connection) in _connectionsByReference)
            {
                connection.Dispose();
            }

            _connectionsByReference.Clear();
        }
    }

    private static IModel CreateChannel(IConnection conn, ConnectionSpec spec)
    {
        var channel = conn.CreateModel();
        channel.BasicQos(spec.PrefetchSize, spec.PrefetchCount, false);

        return channel;
    }
}

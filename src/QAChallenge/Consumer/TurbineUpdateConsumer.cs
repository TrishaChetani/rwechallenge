using QAChallenge.MasterData;
using QAChallenge.Models;
using QAChallenge.RabbitMQ;

namespace QAChallenge.Consumer;

public class TurbineUpdateConsumer : IMessageConsumer<TurbineUpdateInput>
{
    private readonly ILogger<TurbineUpdateConsumer> _logger;
    private readonly IMessagePublisher<TurbineUpdateOutput> _outputPublisher;
    private readonly IMasterDataServiceClient _masterDataClient;

    public TurbineUpdateConsumer(
        ILogger<TurbineUpdateConsumer> logger,
        IMessagePublisher<TurbineUpdateOutput> outputPublisher,
        IMasterDataServiceClient masterDataClient)
    {
        _logger = logger;
        _outputPublisher = outputPublisher;
        _masterDataClient = masterDataClient;
    }

    public async Task<MessageResult> HandleAsync(Message<TurbineUpdateInput> message, CancellationToken token)
    {
        var turbine = await _masterDataClient.GetTurbineByIdAsync(message.Body.Id, token);
        _outputPublisher.Publish(new TurbineUpdateOutput
        {
            Id = turbine.Id.GetValueOrDefault(),
            Name = turbine.Name,
            MaxCapacity = turbine.MaxCapacity.GetValueOrDefault(),
            CreatedAt = turbine.CreatedAt.GetValueOrDefault(),
            LastModifiedAt = turbine.CreatedAt.GetValueOrDefault()
        });

        return MessageResult.Ack();
    }
}

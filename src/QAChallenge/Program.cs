using Microsoft.Extensions.Options;
using QAChallenge.Config;
using QAChallenge.Consumer;
using QAChallenge.MasterData;
using QAChallenge.Models;
using QAChallenge.RabbitMQ.Extensions;
using QAChallenge.RabbitMQ.Models;
using Serilog;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureLogging((context, loggingBuilder) =>
{
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .CreateLogger();

    loggingBuilder.ClearProviders();
    loggingBuilder.AddSerilog();
});

builder.ConfigureServices((context, services) =>
{
    services.Configure<MasterDataConfig>(context.Configuration.GetSection("MasterData"));

    services.AddHttpClient<IMasterDataServiceClient, MasterDataServiceClient>((client, sp) =>
    {
        var cfg = sp.GetRequiredService<IOptions<MasterDataConfig>>();
        client.BaseAddress = cfg.Value.BaseUri;
        return new MasterDataServiceClient(client);
    });

    services.ConfigureRabbitMqConnectionConfigs(context.Configuration);

    var consumerSpec = context.Configuration
        .GetSection("RabbitMq:Consumers:Upstream")
        .Get<ConsumerRegistrationSpec>();

    var publisherSpec = context.Configuration
        .GetSection("RabbitMq:Publishers:Downstream")
        .Get<PublisherRegistrationSpec>();

    services.AddMessageConsumer<TurbineUpdateInput>()
        .WithRegistrationSpec(consumerSpec)
        .RegisterJsonConsumer<TurbineUpdateConsumer>(messageTypeInfo: MessagesJsonContext.Default.TurbineUpdateInput);

    services.AddMessagePublisher<TurbineUpdateOutput>()
        .WithRegistrationSpec(publisherSpec)
        .RegisterJsonPublisher(messageTypeInfo: MessagesJsonContext.Default.TurbineUpdateOutput);
});

var app = builder.Build();

await app.RunAsync();

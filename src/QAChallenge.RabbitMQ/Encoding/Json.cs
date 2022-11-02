using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace QAChallenge.RabbitMQ.Encoding;

public class Json<TBody> : IMessageDecoder<TBody>, IMessageEncoder<TBody> where TBody : notnull
{
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly JsonTypeInfo<TBody>? _jsonType;

    public Json(JsonSerializerOptions? serializerOptions = null, JsonTypeInfo<TBody>? jsonType = null)
    {
        _jsonType = jsonType;
        _serializerOptions = serializerOptions ?? new JsonSerializerOptions(JsonSerializerDefaults.Web);
    }

    public TBody? Deserialize(ReadOnlyMemory<byte> body) => _jsonType is null
        ? JsonSerializer.Deserialize<TBody>(body.Span, _serializerOptions)
        : JsonSerializer.Deserialize(body.Span, _jsonType);

    public ReadOnlyMemory<byte> Encode(TBody body) => _jsonType is null
        ? JsonSerializer.SerializeToUtf8Bytes(body, _serializerOptions)
        : JsonSerializer.SerializeToUtf8Bytes(body, _jsonType);

    public string ContentType => MediaTypeNames.Application.Json;
}

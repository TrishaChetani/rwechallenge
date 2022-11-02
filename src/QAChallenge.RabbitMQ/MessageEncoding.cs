namespace QAChallenge.RabbitMQ;

public interface IMessageDecoder<out TBody> where TBody : notnull
{
    TBody? Deserialize(ReadOnlyMemory<byte> body);
}

public interface IMessageEncoder<in TBody> where TBody : notnull
{
    ReadOnlyMemory<byte> Encode(TBody body);
    string ContentType { get; }
}

using System.Text.Json.Serialization;

namespace QAChallenge.Models;

[JsonSerializable(typeof(TurbineUpdateInput))]
[JsonSerializable(typeof(TurbineUpdateOutput))]
public partial class MessagesJsonContext : JsonSerializerContext
{

}

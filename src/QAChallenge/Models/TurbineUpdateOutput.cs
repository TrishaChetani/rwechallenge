namespace QAChallenge.Models;

public class TurbineUpdateOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public long MaxCapacity { get; set; }
    public DateTimeOffset LastModifiedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

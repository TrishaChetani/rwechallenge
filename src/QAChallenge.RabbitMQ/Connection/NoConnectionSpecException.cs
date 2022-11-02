namespace QAChallenge.RabbitMQ.Connection;

public class NoConnectionSpecException : Exception
{
    public NoConnectionSpecException()
    {
    }

    public NoConnectionSpecException(string connectionReference) : base($"No connection reference {connectionReference} configured")
    {
    }
}

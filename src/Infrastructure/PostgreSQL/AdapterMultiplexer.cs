using StackExchange.Redis;

namespace Infrastructure.PostgreSQL;

public class AdapterMultiplexer : IAdapterMultiplexer
{
    public ConnectionMultiplexer getMultiplexer()
    {
        var connection = Environment.GetEnvironmentVariable("REDIS_CONNECTION");
        if (string.IsNullOrWhiteSpace(connection))
        {
            connection = "localhost:6380";
        }
        return ConnectionMultiplexer.Connect(connection);
    }
}

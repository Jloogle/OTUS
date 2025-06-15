using StackExchange.Redis;

namespace Infrastructure.PostgreSQL;

public class AdapterMultiplexer : IAdapterMultiplexer
{
    public ConnectionMultiplexer getMultiplexer() => ConnectionMultiplexer.Connect("localhost:6380");
}
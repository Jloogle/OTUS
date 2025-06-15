using StackExchange.Redis;

namespace Infrastructure.PostgreSQL;

public interface IAdapterMultiplexer
{
    public ConnectionMultiplexer getMultiplexer();
}
using Domain.Repositories;
using Infrastructure.PostgreSQL;

namespace Infrastructure.Redis.Repository;

/// <inheritdoc />
public class RadisRepositoty(IAdapterMultiplexer adapterMultiplexer) : IRadisRepository
{
    public void StringSet (string key, string value) => adapterMultiplexer.getMultiplexer().GetDatabase().StringSet(key, value);
    public string StringGet (string key) => adapterMultiplexer.getMultiplexer().GetDatabase().StringGet(key).ToString();
}
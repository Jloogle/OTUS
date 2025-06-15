using StackExchange.Redis;

namespace Domain.Repositories;

public interface IRadisRepository 
{
    public void StringSet (string key, string value);
    public string StringGet (string key);
}
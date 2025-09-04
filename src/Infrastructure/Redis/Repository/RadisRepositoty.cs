using Domain.Repositories;
using Infrastructure.PostgreSQL;

namespace Infrastructure.Redis.Repository;

/// <summary>
/// Репозиторий для работы с Redis.
/// </summary>
public class RadisRepositoty(IAdapterMultiplexer adapterMultiplexer) : IRadisRepository
{
    /// <summary>
    /// Установить строковое значение по ключу в Redis.
    /// </summary>
    /// <param name="key">Ключ для сохранения значения.</param>
    /// <param name="value">Строковое значение для сохранения.</param>
    public void StringSet (string key, string value) => adapterMultiplexer.getMultiplexer().GetDatabase().StringSet(key, value);
    /// <summary>
    /// Получить строковое значение по ключу в Redis.
    /// </summary>
    /// <param name="key">Ключ для получения значения.</param>
    /// <returns>Строковое значение.</returns>
    public string StringGet (string key) => adapterMultiplexer.getMultiplexer().GetDatabase().StringGet(key).ToString();
    /// <summary>
    /// Удалить строковое значение по ключу в Redis.
    /// </summary>
    /// <param name="key">Ключ для удаления значения.</param>
    public void StringDelete(string key) => adapterMultiplexer.getMultiplexer().GetDatabase().KeyDelete(key);
}
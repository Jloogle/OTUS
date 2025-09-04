using StackExchange.Redis;

namespace Domain.Repositories;

/// <summary>
/// Минимальная абстракция Redis для простых операций со строками.
/// </summary>
public interface IRadisRepository
{
    /// <summary>
    /// Установить строковое значение по ключу в Redis.
    /// </summary>
    /// <param name="key">Ключ для сохранения значения.</param>
    /// <param name="value">Строковое значение для сохранения.</param>
    public void StringSet(string key, string value);

    /// <summary>
    /// Получить строковое значение по ключу из Redis.
    /// </summary>
    /// <param name="key">Ключ для получения значения.</param>
    /// <returns>Строковое значение или пустая строка, если ключ не найден.</returns>
    public string StringGet(string key);

    /// <summary>
    /// Удалить ключ из Redis.
    /// </summary>
    /// <param name="key">Ключ для удаления.</param>
    public void StringDelete(string key);
}

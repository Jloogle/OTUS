using Domain.Entities;

namespace Domain.Repositories;

/// <summary>
/// Репозиторий для поиска ролей.
/// </summary>
public interface IRoleRepository
{
    /// <summary>
    /// Получить роль по имени или null, если не найдена.
    /// </summary>
    /// <param name="roleName">Название роли для поиска.</param>
    /// <returns>Найденная роль или null, если не найдена.</returns>
    Task<Role?> GetByNameAsync(string roleName);
}

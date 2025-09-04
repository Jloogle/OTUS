using Domain.Entities;

namespace Domain.Repositories
{
    /// <summary>
    /// Репозиторий для работы с пользователями
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Найти пользователя по имени.
        /// </summary>
        /// <param name="name">Имя пользователя для поиска.</param>
        /// <returns>Найденный пользователь или null, если не найден.</returns>
        Task<User> FindByNameAsync(string name);

        /// <summary>
        /// Найти пользователя по электронной почте.
        /// </summary>
        /// <param name="email">Адрес электронной почты для поиска.</param>
        /// <returns>Найденный пользователь или null, если не найден.</returns>
        Task<User> FindByEmailAsync(string email);

        /// <summary>
        /// Получить всех пользователей с определенной ролью.
        /// </summary>
        /// <param name="roleName">Название роли.</param>
        /// <returns>Список пользователей с указанной ролью.</returns>
        Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName);

        /// <summary>
        /// Получить все проекты пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Список проектов пользователя.</returns>
        Task<IEnumerable<Project?>> GetUserProjectsAsync(int userId);

        /// <summary>
        /// Добавить роль пользователю.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="roleId">Идентификатор роли.</param>
        Task AddRoleToUserAsync(int userId, int roleId);

        /// <summary>
        /// Удалить роль у пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="roleId">Идентификатор роли.</param>
        Task RemoveRoleFromUserAsync(int userId, int roleId);
        /// <summary>
        /// Добавить нового пользователя в систему.
        /// </summary>
        /// <param name="user">Данные пользователя для добавления.</param>
        /// <returns>Задача, представляющая асинхронную операцию.</returns>
        Task AddUser(User user);

        /// <summary>
        /// Найти пользователя по Telegram ID.
        /// </summary>
        /// <param name="id">Telegram ID пользователя.</param>
        /// <returns>Найденный пользователь или null, если не найден.</returns>
        public Task<User> FindByIdTelegram(long? id);

        /// <summary>
        /// Найти пользователя по Telegram ID с загрузкой связанных данных (роли и проекты).
        /// </summary>
        /// <param name="id">Telegram ID пользователя.</param>
        /// <returns>Найденный пользователь с загруженными данными или null, если не найден.</returns>
        Task<User?> FindByIdTelegramWithDetails(long? id);
    }
}

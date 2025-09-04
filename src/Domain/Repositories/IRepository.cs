using System.Linq.Expressions;

namespace Domain.Repositories
{
    /// <summary>
    /// Общий интерфейс репозитория для работы с сущностями
    /// </summary>
    /// <typeparam name="T">Тип сущности</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Получить сущность по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор сущности.</param>
        /// <returns>Найденная сущность или null, если не найдена.</returns>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// Получить все сущности данного типа.
        /// </summary>
        /// <returns>Список всех сущностей.</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Найти сущности по условию.
        /// </summary>
        /// <param name="predicate">Условие для поиска сущностей.</param>
        /// <returns>Список сущностей, удовлетворяющих условию.</returns>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Добавить новую сущность в репозиторий.
        /// </summary>
        /// <param name="entity">Сущность для добавления.</param>
        /// <returns>Задача, представляющая асинхронную операцию.</returns>
        Task AddAsync(T entity);

        /// <summary>
        /// Обновить существующую сущность.
        /// </summary>
        /// <param name="entity">Сущность с обновленными данными.</param>
        /// <returns>Задача, представляющая асинхронную операцию.</returns>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Удалить сущность из репозитория.
        /// </summary>
        /// <param name="entity">Сущность для удаления.</param>
        /// <returns>Задача, представляющая асинхронную операцию.</returns>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Удалить сущность по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор сущности для удаления.</param>
        /// <returns>Задача, представляющая асинхронную операцию.</returns>
        Task DeleteByIdAsync(int id);

        /// <summary>
        /// Сохранить все изменения в базе данных.
        /// </summary>
        /// <returns>Задача, представляющая асинхронную операцию.</returns>
        Task SaveChangesAsync();
    }
}
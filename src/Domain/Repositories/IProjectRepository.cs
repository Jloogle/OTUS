using Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Domain.Repositories
{
    /// <summary>
    /// Репозиторий для работы с проектами
    /// </summary>
    public interface IProjectRepository : IRepository<Project>
    {
        /// <summary>
        /// Найти проект по названию.
        /// </summary>
        /// <param name="name">Название проекта для поиска.</param>
        /// <returns>Найденный проект или null, если не найден.</returns>
        Task<Project> FindByNameAsync(string name);

        /// <summary>
        /// Получить все задачи проекта.
        /// </summary>
        /// <param name="projectId">Идентификатор проекта.</param>
        /// <returns>Список задач проекта.</returns>
        Task<IEnumerable<ProjTask>> GetProjectTasksAsync(int projectId);

        /// <summary>
        /// Получить всех участников проекта.
        /// </summary>
        /// <param name="projectId">Идентификатор проекта.</param>
        /// <returns>Список участников проекта.</returns>
        Task<IEnumerable<User>> GetProjectMembersAsync(int projectId);

        /// <summary>
        /// Добавить пользователя к проекту.
        /// </summary>
        /// <param name="projectId">Идентификатор проекта.</param>
        /// <param name="userId">Идентификатор пользователя.</param>
        Task AddUserToProjectAsync(int projectId, int userId);

        /// <summary>
        /// Удалить пользователя из проекта.
        /// </summary>
        /// <param name="projectId">Идентификатор проекта.</param>
        /// <param name="userId">Идентификатор пользователя.</param>
        Task RemoveUserFromProjectAsync(int projectId, int userId);

        /// <summary>
        /// Асинхронно добавляет новый проект в базу данных.
        /// </summary>
        /// <param name="name">Название проекта для добавления. Не должно быть null или пустым.</param>
        /// <param name="deadline">Дедлайн завершения проекта.</param>
        /// <returns>
        /// Задача, представляющая асинхронную операцию. Результат задачи содержит
        /// количество записей состояния, записанных в базу данных.
        /// </returns>
        public Task<int> AddProjectAsync(string name, DateTime deadline);

        /// <summary>
        /// Удаляет проект по идентификатору.
        /// </summary>
        /// <param name="projectId">Идентификатор проекта для удаления.</param>
        public Task RemoveProjectAsync(int projectId);

        /// <summary>
        /// Получить все проекты пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Список проектов пользователя.</returns>
        Task<IEnumerable<Project>> GetUserProjectsAsync(int userId);
    }
}
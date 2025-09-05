using Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Domain.Repositories
{
    /// <summary>
    /// Репозиторий для работы с задачами
    /// </summary>
    public interface ITaskRepository : IRepository<ProjTask>
    {
        /// <summary>
        /// Получить все задачи проекта.
        /// </summary>
        /// <param name="projectId">Идентификатор проекта.</param>
        /// <returns>Список задач проекта.</returns>
        Task<IEnumerable<ProjTask>> GetTasksByProjectAsync(int projectId);

        /// <summary>
        /// Получить все задачи пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Список задач пользователя.</returns>
        Task<IEnumerable<ProjTask>> GetTasksByUserAsync(int userId);

        /// <summary>
        /// Изменить статус задачи.
        /// </summary>
        /// <param name="taskId">Идентификатор задачи.</param>
        /// <param name="status">Новый статус задачи.</param>
        Task ChangeTaskStatusAsync(int taskId, string status);

        /// <summary>
        /// Назначить задачу пользователю.
        /// </summary>
        /// <param name="taskId">Идентификатор задачи.</param>
        /// <param name="userId">Идентификатор пользователя.</param>
        Task AssignTaskToUserAsync(int taskId, int userId);

        /// <summary>
        /// Удалить задачу по идентификатору.
        /// </summary>
        /// <param name="taskId">Идентификатор задачи для удаления.</param>
        /// <returns>Задача, представляющая асинхронную операцию.</returns>
        public Task DeleteTaskAsync(int taskId);

        /// <summary>
        /// Добавить новую задачу в проект.
        /// </summary>
        /// <param name="task">Данные задачи для добавления.</param>
        /// <param name="projectId">Идентификатор проекта.</param>
        /// <returns>Добавленная задача.</returns>
        public Task<ProjTask> AddTaskAsync(ProjTask task, int projectId);

        /// <summary>
        /// Обновить существующую задачу.
        /// </summary>
        /// <param name="updatedTask">Обновленные данные задачи.</param>
        /// <returns>Обновленная задача.</returns>
        public Task<ProjTask> UpdateTaskAsync(ProjTask updatedTask);

        /// <summary>
        /// Получить все задачи в системе.
        /// </summary>
        /// <returns>Список всех задач.</returns>
        Task<IEnumerable<ProjTask>> GetAllTasksAsync();

        /// <summary>
        /// Получить задачи пользователя по Telegram ID.
        /// </summary>
        /// <param name="userId">Telegram ID пользователя.</param>
        /// <returns>Список задач пользователя.</returns>
        Task<IEnumerable<ProjTask>> GetTasksByUserIdAsync(long? userId);

        /// <summary>
        /// Получить задачу с подгруженным проектом по ID.
        /// </summary>
        Task<ProjTask?> GetTaskWithProjectAsync(int taskId);
    }
}

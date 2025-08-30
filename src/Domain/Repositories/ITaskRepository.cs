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
        /// Получить задачи по проекту
        /// </summary>
        Task<IEnumerable<ProjTask>> GetTasksByProjectAsync(int projectId);
        
        /// <summary>
        /// Получить задачи пользователя
        /// </summary>
        Task<IEnumerable<ProjTask>> GetTasksByUserAsync(int userId);
        
        /// <summary>
        /// Изменить статус задачи
        /// </summary>
        Task ChangeTaskStatusAsync(int taskId, string status);
        
        /// <summary>
        /// Назначить задачу пользователю
        /// </summary>
        Task AssignTaskToUserAsync(int taskId, int userId);

        /// <summary>
        /// Удалить задачу по ID
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public Task DeleteTaskAsync(int taskId);

        public Task<ProjTask> AddTaskAsync(ProjTask task, int projectId);

        public Task<ProjTask> UpdateTaskAsync(ProjTask updatedTask);
    }
} 
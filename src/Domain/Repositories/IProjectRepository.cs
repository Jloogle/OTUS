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
        /// Найти проект по названию
        /// </summary>
        Task<Project> FindByNameAsync(string name);
        
        /// <summary>
        /// Получить задачи проекта
        /// </summary>
        Task<IEnumerable<ProjTask>> GetProjectTasksAsync(int projectId);
        
        /// <summary>
        /// Получить участников проекта
        /// </summary>
        Task<IEnumerable<User>> GetProjectMembersAsync(int projectId);
        
        /// <summary>
        /// Добавить пользователя к проекту
        /// </summary>
        Task AddUserToProjectAsync(int projectId, int userId);
        
        /// <summary>
        /// Удалить пользователя из проекта
        /// </summary>
        Task RemoveUserFromProjectAsync(int projectId, int userId);

        /// <summary>
        /// Asynchronously adds a new project to the database.
        /// </summary>
        /// <param name="name">The name of the project to be added. Must not be null or empty.</param>
        /// <param name="deadline">The deadline for the project completion.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains
        /// the number of state entries written to the database.
        /// </returns>
        public Task<int> AddProjectAsync(string name, DateTime deadline);

        public Task RemoveProjectAsync(int projectId);
        
        /// <summary>
        /// Получить проекты пользователя
        /// </summary>
        Task<IEnumerable<Project>> GetUserProjectsAsync(int userId);
    }
} 
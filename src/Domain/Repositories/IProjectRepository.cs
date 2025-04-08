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
    }
} 
using Domain.Entities;

namespace Domain.Repositories
{
    /// <summary>
    /// Репозиторий для работы с пользователями
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Найти пользователя по имени
        /// </summary>
        Task<User> FindByNameAsync(string name);
        
        /// <summary>
        /// Найти пользователя по электронной почте
        /// </summary>
        Task<User> FindByEmailAsync(string email);
        
        /// <summary>
        /// Получить пользователей с определенной ролью
        /// </summary>
        Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName);
        
        /// <summary>
        /// Получить проекты пользователя
        /// </summary>
        Task<IEnumerable<Project>> GetUserProjectsAsync(int userId);
        
        /// <summary>
        /// Добавить роль пользователю
        /// </summary>
        Task AddRoleToUserAsync(int userId, int roleId);
        
        /// <summary>
        /// Удалить роль у пользователя
        /// </summary>
        Task RemoveRoleFromUserAsync(int userId, int roleId);
        Task AddUser(User user);
    }
} 
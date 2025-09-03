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
        Task<IEnumerable<Project?>> GetUserProjectsAsync(int userId);
        
        /// <summary>
        /// Добавить роль пользователю
        /// </summary>
        Task AddRoleToUserAsync(int userId, int roleId);
        
        /// <summary>
        /// Удалить роль у пользователя
        /// </summary>
        Task RemoveRoleFromUserAsync(int userId, int roleId);
        /// <summary>
        /// Добавить пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task AddUser(User user);
        /// <summary>
        /// Найти пользователя по id телеграма
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<User> FindByIdTelegram(long? id);

        /// <summary>
        /// Найти пользователя по Telegram Id с загрузкой ролей и проектов
        /// </summary>
        Task<User?> FindByIdTelegramWithDetails(long? id);
    }
} 

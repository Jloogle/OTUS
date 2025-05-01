using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    /// <summary>
    /// Общий интерфейс репозитория для работы с сущностями
    /// </summary>
    /// <typeparam name="T">Тип сущности</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Получить сущность по идентификатору
        /// </summary>
        Task<T> GetByIdAsync(int id);
        
        /// <summary>
        /// Получить все сущности
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();
        
        /// <summary>
        /// Найти сущности по условию
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        
        /// <summary>
        /// Добавить сущность
        /// </summary>
        Task AddAsync(T entity);
        
        /// <summary>
        /// Обновить сущность
        /// </summary>
        Task UpdateAsync(T entity);
        
        /// <summary>
        /// Удалить сущность
        /// </summary>
        Task DeleteAsync(T entity);
        
        /// <summary>
        /// Удалить сущность по идентификатору
        /// </summary>
        Task DeleteByIdAsync(int id);
        
        /// <summary>
        /// Сохранить изменения
        /// </summary>
        Task SaveChangesAsync();
    }
} 
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.PostgreSQL.Repository
{
    /// <summary>
    /// Базовая реализация репозитория с использованием Entity Framework Core
    /// </summary>
    public abstract class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationContext _context;
        protected readonly DbSet<T> _dbSet;
        protected readonly ILogger<BaseRepository<T>> _logger;

        protected BaseRepository(ApplicationContext context, ILogger<BaseRepository<T>> logger = null)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<T>();
            _logger = logger;
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            try
            {
                return await _dbSet.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Ошибка при получении {typeof(T).Name} по ID: {id}");
                throw;
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                return await _dbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Ошибка при получении всех {typeof(T).Name}");
                throw;
            }
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await _dbSet.Where(predicate).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Ошибка при поиске {typeof(T).Name} по предикату");
                throw;
            }
        }

        public virtual async Task AddAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
                
            try
            {
                await _dbSet.AddAsync(entity);
                _logger?.LogInformation($"Добавлена новая сущность {typeof(T).Name}");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Ошибка при добавлении {typeof(T).Name}");
                throw;
            }
        }

        public virtual Task UpdateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
                
            try
            {
                _context.Entry(entity).State = EntityState.Modified;
                _logger?.LogInformation($"Обновлена сущность {typeof(T).Name}");
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Ошибка при обновлении {typeof(T).Name}");
                throw;
            }
        }

        public virtual Task DeleteAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
                
            try
            {
                _dbSet.Remove(entity);
                _logger?.LogInformation($"Удалена сущность {typeof(T).Name}");
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Ошибка при удалении {typeof(T).Name}");
                throw;
            }
        }

        public virtual async Task DeleteByIdAsync(int id)
        {
            try
            {
                var entity = await GetByIdAsync(id);
                if (entity != null)
                {
                    await DeleteAsync(entity);
                    _logger?.LogInformation($"Удалена сущность {typeof(T).Name} с ID: {id}");
                }
                else
                {
                    _logger?.LogWarning($"Не удалось найти сущность {typeof(T).Name} с ID: {id} для удаления");
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Ошибка при удалении {typeof(T).Name} по ID: {id}");
                throw;
            }
        }

        public virtual async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                _logger?.LogInformation($"Сохранены изменения для {typeof(T).Name}");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger?.LogError(ex, $"Конфликт параллельного обновления для {typeof(T).Name}");
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger?.LogError(ex, $"Ошибка при сохранении изменений для {typeof(T).Name}");
                throw;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Непредвиденная ошибка при сохранении изменений для {typeof(T).Name}");
                throw;
            }
        }
    }
} 
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSQL.Repository
{
    public class UserRepository(IAdapterApplicationContext context) : BaseRepository<User>(context), IUserRepository
    {
        public async Task<User> FindByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Имя пользователя не может быть пустым", nameof(name));
                
            return await _dbSet.FirstOrDefaultAsync(u => u.Name == name);
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Email пользователя не может быть пустым", nameof(email));
                
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentException("Название роли не может быть пустым", nameof(roleName));
                
            return await _dbSet
                .Include(u => u.Role)
                .Where(u => u.Role.Any(r => r.Name == roleName))
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetUserProjectsAsync(int userId)
        {
            var user = await _dbSet
                .Include(u => u.Projects)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new InvalidOperationException($"Пользователь с ID {userId} не найден");
                
            return user.Projects ?? new List<Project>();
        }
        
        /// <summary>
        /// Добавляет роль пользователю
        /// </summary>
        public async Task AddRoleToUserAsync(int userId, int roleId)
        {
            var user = await _dbSet
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new InvalidOperationException($"Пользователь с ID {userId} не найден");
                
            var role = await _context.Roles.FindAsync(roleId)
                ?? throw new InvalidOperationException($"Роль с ID {roleId} не найдена");
                
            // Проверяем, есть ли уже такая роль у пользователя
            if (user.Role.Any(r => r.Id == roleId))
                return;
                
            // Используем транзакцию
            using var transaction = await _context.Database.BeginTransactionAsync();
            try 
            {
                user.Role.Add(role);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        
        /// <summary>
        /// Удаляет роль у пользователя
        /// </summary>
        public async Task RemoveRoleFromUserAsync(int userId, int roleId)
        {
            var user = await _dbSet
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new InvalidOperationException($"Пользователь с ID {userId} не найден");
                
            var role = user.Role.FirstOrDefault(r => r.Id == roleId)
                ?? throw new InvalidOperationException($"У пользователя нет роли с ID {roleId}");
                
            // Используем транзакцию
            using var transaction = await _context.Database.BeginTransactionAsync();
            try 
            {
                user.Role.Remove(role);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task AddUser(User user)
        {
            _context.Users.AddRange(user);
            await _context.SaveChangesAsync();
        }
        
        public async Task<User> FindByIdTelegram(long? id)
        {
            if (id==null)
                throw new ArgumentException("Telegram ID не может быть пустым", nameof(id));
                
            return (await _dbSet.FirstOrDefaultAsync(u => u.IdTelegram == id))!;
        }
    }
} 
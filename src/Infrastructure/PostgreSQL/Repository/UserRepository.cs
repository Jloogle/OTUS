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

        public Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName)
        {
            throw new NotImplementedException();
        }


        public async Task<IEnumerable<Project?>> GetUserProjectsAsync(int userId)
        {
            var user = await _dbSet
                .Include(u => u.Projects)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new InvalidOperationException($"Пользователь с ID {userId} не найден");
                
            return user.Projects ?? new List<Project?>();
        }

        public Task AddRoleToUserAsync(int userId, int roleId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRoleFromUserAsync(int userId, int roleId)
        {
            throw new NotImplementedException();
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
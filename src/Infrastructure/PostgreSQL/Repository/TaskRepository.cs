
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.PostgreSQL.Repository;

public class TaskRepository : BaseRepository<ProjTask>, ITaskRepository
{
    public TaskRepository(IAdapterApplicationContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ProjTask>> GetTasksByProjectAsync(int projectId)
    {
        return await _dbSet
            .Where(t => t.Project.Id == projectId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProjTask>> GetTasksByUserAsync(int userId)
    {
        // Так как в ProjTask нет прямой связи с User, предполагаем, что задачи проекта
        // нужно получить через проекты пользователя
        var user = await _context.Users
            .Include(u => u.Projects)
            .ThenInclude(p => p.ProjTasks)
            .FirstOrDefaultAsync(u => u.Id == userId);
            
        if (user == null)
            return new List<ProjTask>();
            
        // Собираем все задачи из всех проектов пользователя
        return user.Projects.SelectMany(p => p.ProjTasks).ToList();
    }
    
    public async Task<ProjTask> AddTaskAsync(ProjTask task, int projectId)
    {
        if (task == null)
            throw new ArgumentNullException(nameof(task));
    
        var project = await _context.Projects.FindAsync(projectId)
                      ?? throw new InvalidOperationException($"Проект с ID {projectId} не найден");
    
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Привязываем задачу к проекту
            task.Project = project;
            
            // Добавляем задачу
            await _dbSet.AddAsync(task);
    
            // Создаем уведомление о создании задачи
            var notification = new Notification
            {
                Name = "Создание задачи",
                Age = DateTime.UtcNow,
                Description = $"Создана новая задача '{task.Name}' в проекте '{project.Name}'"
            };
    
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
    
            // Добавляем связь между уведомлением и задачей после сохранения
            notification.Tasks.Add(task);
            await _context.SaveChangesAsync();
            
            await transaction.CommitAsync();
            return task;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ProjTask> UpdateTaskAsync(ProjTask updatedTask)
    {
        if (updatedTask == null)
            throw new ArgumentNullException(nameof(updatedTask));
        
        _context.Tasks.Update(updatedTask);
        await _context.SaveChangesAsync();
        return updatedTask;

    }

    public async Task ChangeTaskStatusAsync(int taskId, string status)
    {
        if (string.IsNullOrEmpty(status))
            throw new ArgumentException("Статус задачи не может быть пустым", nameof(status));

        var task = await _dbSet.FindAsync(taskId)
            ?? throw new InvalidOperationException($"Задача с ID {taskId} не найдена");

        // Используем транзакцию для обеспечения целостности
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Так как в ProjTask нет поля Status, добавим его как свойство
            // В идеале нужно обновить модель данных или использовать словарь свойств
            task.Description = $"Статус: {status}. {task.Description}";
            
            // Создаем уведомление о смене статуса задачи
            var notification = new Notification
            {
                Name = $"Изменение статуса задачи",
                Age = DateTime.UtcNow,
                Description = $"Статус задачи '{task.Name}' изменен на '{status}'",
            };
            
            notification.Tasks.Add(task);
            
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task AssignTaskToUserAsync(int taskId, int userId)
    {
        var task = await _dbSet
            .Include(t => t.Project)
            .Include(t => t.AssignedUsers)
            .FirstOrDefaultAsync(t => t.Id == taskId)
            ?? throw new InvalidOperationException($"Задача с ID {taskId} не найдена");
            
        var user = await _context.Users.FindAsync(userId);
        if (user == null && userId != 0) // userId = 0 означает "снять назначение"
            throw new InvalidOperationException($"Пользователь с ID {userId} не найден");
            
        if (task.Project == null)
            throw new InvalidOperationException($"Задача с ID {taskId} не привязана к проекту");
    
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            string message;
            if (userId == 0)
            {
                // Снимаем все назначения с задачи
                task.AssignedUsers.Clear();
                message = $"Задача '{task.Name}' снята с назначения";
            }
            else
            {
                // Проверяем, не назначена ли уже задача этому пользователю
                if (!task.AssignedUsers.Any(u => u.Id == userId))
                {
                    task.AssignedUsers.Add(user);
                    message = $"Задача '{task.Name}' назначена пользователю {user.Name}";
                }
                else
                {
                    message = $"Задача '{task.Name}' уже назначена пользователю {user.Name}";
                }
            }
            
            // Сохраняем изменения в задаче
            _context.Tasks.Update(task);
            
            // Создаем уведомление о назначении задачи
            var notification = new Notification
            {
                Name = "Назначение задачи",
                Age = DateTime.UtcNow,
                Description = message
            };
            
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
            
            // Добавляем связь между уведомлением и задачей после сохранения
            notification.Tasks.Add(task);
            await _context.SaveChangesAsync();
            
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeleteTaskAsync(int taskId)
    {
        var task = await _dbSet
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == taskId)
            ?? throw new InvalidOperationException($"Задача с ID {taskId} не найдена");

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {

            // Создаем уведомление об удалении задачи
            var deleteNotification = new Notification
            {
                Name = "Удаление задачи",
                Age = DateTime.UtcNow,
                Description = $"Задача '{task.Name}' была удалена из проекта '{task.Project?.Name}'"
            };

            await _context.Notifications.AddAsync(deleteNotification);

            // Удаляем задачу
            _context.Tasks.Remove(task);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    // Добавим новые методы для работы с назначениями
    public async Task<IEnumerable<User>> GetTaskAssignedUsersAsync(int taskId)
    {
        var task = await _dbSet
            .Include(t => t.AssignedUsers)
            .FirstOrDefaultAsync(t => t.Id == taskId);
        
        return task?.AssignedUsers ?? new List<User>();
    }

    public async Task<IEnumerable<ProjTask>> GetUserAssignedTasksAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.AssignedTasks)
            .ThenInclude(t => t.Project)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        return user?.AssignedTasks ?? new List<ProjTask>();
    }

    public async Task RemoveUserFromTaskAsync(int taskId, int userId)
    {
        var task = await _dbSet
            .Include(t => t.AssignedUsers)
            .FirstOrDefaultAsync(t => t.Id == taskId)
            ?? throw new InvalidOperationException($"Задача с ID {taskId} не найдена");

        var userToRemove = task.AssignedUsers.FirstOrDefault(u => u.Id == userId);
        if (userToRemove != null)
        {
            task.AssignedUsers.Remove(userToRemove);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task<IEnumerable<ProjTask>> GetAllTasksAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IEnumerable<ProjTask>> GetTasksByUserIdAsync(long? IdTelegram)
    {
        var user = await _context.Users
            .Include(u => u.Projects)
            .ThenInclude(p => p.ProjTasks)
            .FirstOrDefaultAsync(u => u.IdTelegram == IdTelegram);
        
        if (user == null)
            return new List<ProjTask>();
        
        return user.Projects.SelectMany(p => p.ProjTasks).ToList();
    }
}

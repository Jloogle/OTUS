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
            .FirstOrDefaultAsync(t => t.Id == taskId)
            ?? throw new InvalidOperationException($"Задача с ID {taskId} не найдена");
            
        var user = await _context.Users.FindAsync(userId);
        if (user == null && userId != 0) // userId = 0 означает "снять назначение"
            throw new InvalidOperationException($"Пользователь с ID {userId} не найден");
            
        if (task.Project == null)
            throw new InvalidOperationException($"Задача с ID {taskId} не привязана к проекту");

        // Используем транзакцию для обеспечения целостности
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Сначала проверяем, относится ли пользователь к проекту
            var project = await _context.Projects
                .Include(p => p.Users)
                .FirstOrDefaultAsync(p => p.Id == task.Project.Id);
                
            if (userId != 0 && !project.Users.Any(u => u.Id == userId))
            {
                // Если пользователя нет в проекте, добавляем его
                project.Users.Add(user);
            }
            
            string message;
            if (userId == 0)
            {
                message = $"Задача '{task.Name}' снята с назначения";
                // В данной модели нет прямого свойства AssigneeId, оставляем примечание в Description
                task.Description = $"[Снято назначение] {task.Description}";
            }
            else
            {
                message = $"Задача '{task.Name}' назначена пользователю {user.Name}";
                // В данной модели нет прямого свойства AssigneeId, оставляем примечание в Description
                task.Description = $"[Назначена для: {user.Name}] {task.Description}";
            }
            
            // Создаем уведомление о назначении задачи
            var notification = new Notification
            {
                Name = "Назначение задачи",
                Age = DateTime.UtcNow,
                Description = message
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
            _dbSet.Remove(task);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
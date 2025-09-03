
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.PostgreSQL.Repository;

public class ProjectRepository : BaseRepository<Project>, IProjectRepository
{
    public ProjectRepository(IAdapterApplicationContext context) : base(context)
    {
    }

    /// <summary>
    /// Найти проект по названию
    /// </summary>
    public async Task<Project> FindByNameAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Название проекта не может быть пустым", nameof(name));
                
        return await _dbSet.FirstOrDefaultAsync(p => p.Name == name) ?? throw new ArgumentException($"Проект с названием '{name}' не найден", nameof(name));
    }
    
    /// <summary>
    /// Asynchronously adds a new project to the database.
    /// </summary>
    /// <param name="name">The name of the project to be added. Must not be null or empty.</param>
    /// <param name="deadline">The deadline for the project completion.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// the number of state entries written to the database.
    /// </returns>
    public async Task<int> AddProjectAsync(string name, DateTime deadline)  
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название проекта не может быть пустым", nameof(name));

        // Convert the deadline to UTC if it's not already
        var utcDeadline = deadline.Kind == DateTimeKind.Utc 
            ? deadline 
            : DateTime.SpecifyKind(deadline, DateTimeKind.Utc);

        var project = new Project { Name = name, Deadline = utcDeadline };
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return project.Id;
    }
    
    /// <summary>
    /// Получить задачи проекта
    /// </summary>
    public async Task<IEnumerable<ProjTask>> GetProjectTasksAsync(int projectId)
    {
        var project = await _dbSet
            .Include(p => p.ProjTasks)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        return project?.ProjTasks ?? new List<ProjTask>();
    }
    
    /// <summary>
    /// Получить участников проекта
    /// </summary>
    public async Task<IEnumerable<User>> GetProjectMembersAsync(int projectId)
    {
        var project = await _dbSet
            .Include(p => p.Users)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        return project?.Users ?? new List<User>();
    }
    
   
    
   
    /// <summary>
    /// Удалить пользователя из проекта
    /// </summary>
    public async Task RemoveProjectAsync(int projectId)
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project != null)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }
    }
    
    /// <summary>
    /// Добавить пользователя в проект
    /// </summary>
    public async Task AddUserToProjectAsync(int projectId, int userId)
    {
        Console.WriteLine($"Добавление пользователя {userId} в проект {projectId}");
        
        var project = await _dbSet
            .Include(p => p.Users)
            .FirstOrDefaultAsync(p => p.Id == projectId);
    
        if (project == null)
        {
             throw new ArgumentException($"Проект с ID {projectId} не найден", nameof(projectId));
        }
    
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            Console.WriteLine($"Пользователь с ID {userId} не найден");
            throw new ArgumentException($"Пользователь с ID {userId} не найден", nameof(userId));
        }
    
        if (project.Users.All(u => u.Id != userId))
        {
            Console.WriteLine($"Добавляем пользователя {user.Name} (ID: {user.Id}) в проект {project.Name} (ID: {project.Id})");
            project.Users.Add(user);
            
            // Проверим состояние перед сохранением
            Console.WriteLine($"Количество пользователей в проекте перед сохранением: {project.Users.Count}");
            
            await _context.SaveChangesAsync();
            Console.WriteLine("Пользователь успешно добавлен в проект");
            
            // Проверим состояние после сохранения
            Console.WriteLine($"Количество пользователей в проекте после сохранения: {project.Users.Count}");
        }
        else
        {
            Console.WriteLine($"Пользователь {userId} уже является участником проекта {projectId}");
        }
    }

    /// <summary>
    /// Удалить пользователя из проекта
    /// </summary>
    public async Task RemoveUserFromProjectAsync(int projectId, int userId)
    {
        var project = await _dbSet
            .Include(p => p.Users)
            .FirstOrDefaultAsync(p => p.Id == projectId);
    
        if (project == null)
            throw new ArgumentException($"Проект с ID {projectId} не найден", nameof(projectId));
    
        var user = project.Users.FirstOrDefault(u => u.Id == userId);
        if (user != null)
        {
            project.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
    
    /// <summary>
    /// Получить проекты пользователя
    /// </summary>
    public async Task<IEnumerable<Project>> GetUserProjectsAsync(int userId)
    {
        Console.WriteLine($"Поиск проектов для пользователя {userId}");
        
        // Попробуем загрузить все проекты и отфильтровать на стороне клиента
        var allProjects = await _dbSet
            .Include(p => p.Users)
            .ToListAsync();
            
        Console.WriteLine($"Всего проектов в базе: {allProjects.Count}");
        
        var userProjects = allProjects
            .Where(p => p.Users.Any(u => u.Id == userId))
            .ToList();
            
        Console.WriteLine($"Найдено проектов для пользователя {userId}: {userProjects.Count}");
        foreach (var project in userProjects)
        {
            Console.WriteLine($"  - Проект: {project.Name} (ID: {project.Id}), участников: {project.Users.Count}");
        }
        
        return userProjects;
    }
}

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
                
        return await _dbSet.FirstOrDefaultAsync(p => p.Name == name);
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
        // Convert the deadline to UTC if it's not already
        var utcDeadline = deadline.Kind == DateTimeKind.Utc 
            ? deadline 
            : DateTime.SpecifyKind(deadline, DateTimeKind.Utc);

        var project1 = new Project { Name = name, Deadline = utcDeadline };
        _context.Projects.AddRange(project1);
        return await _context.SaveChangesAsync();
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
    /// Добавить пользователя к проекту
    /// </summary>
    public async Task AddUserToProjectAsync(int projectId, int userId)
    {
        var project = await _dbSet.FindAsync(projectId) 
            ?? throw new InvalidOperationException($"Проект с ID {projectId} не найден");
                
        var user = await _context.Users.FindAsync(userId)
            ?? throw new InvalidOperationException($"Пользователь с ID {userId} не найден");

        // Используем транзакцию для обеспечения целостности
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (project.Users == null)
                project.Users = new List<User>();
                
            if (!project.Users.Any(u => u.Id == userId))
            {
                project.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    /// <summary>
    /// Удалить пользователя из проекта
    /// </summary>
    public async Task RemoveUserFromProjectAsync(int projectId, int userId)
    {
        var project = await _dbSet
            .Include(p => p.Users)
            .FirstOrDefaultAsync(p => p.Id == projectId)
            ?? throw new InvalidOperationException($"Проект с ID {projectId} не найден");

        var user = project.Users?.FirstOrDefault(u => u.Id == userId)
            ?? throw new InvalidOperationException($"Пользователь с ID {userId} не найден в проекте");

        // Используем транзакцию для обеспечения целостности
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            project.Users.Remove(user);
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
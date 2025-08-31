using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class User
{
    public int Id { get; init; }
    public string? Name { get; set; }
    public int Age { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    [NotMapped]
    public int State { get; set; }
    public long? IdTelegram { get; set; }
    
    // Существующие связи
    public List<Project> Projects { get; set; } = new List<Project>();
    
    // Новая связь многие-ко-многим с задачами
    public List<ProjTask> AssignedTasks { get; set; } = new List<ProjTask>();
}
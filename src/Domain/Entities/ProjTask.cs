namespace Domain.Entities;

public class ProjTask 
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime? Deadline { get; set; }
    
    // Существующие связи
    public virtual Project Project { get; set; } = null!;
    //public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    
    // Новая связь многие-ко-многим с пользователями
    public virtual ICollection<User> AssignedUsers { get; set; } = new List<User>();
}
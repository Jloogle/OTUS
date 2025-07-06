namespace Domain.Entities;

public class Project
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public DateTime Deadline { get; init; }
    public List<User> Users { get; set; } = new List<User>();
    public List<ProjTask> ProjTasks { get; set; } = new List<ProjTask>();
}
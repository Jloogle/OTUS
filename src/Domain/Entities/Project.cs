namespace Domain.Entities;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime Deadline { get; set; }
    public List<User> Users { get; set; } = new List<User>();
    public List<ProjTask> ProjTasks { get; set; } = new List<ProjTask>();
}
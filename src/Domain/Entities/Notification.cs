namespace Application.Dto;

public class Notification
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime Age { get; set; }
    public string Description { get; set; }
    public List<ProjTask> Tasks { get; set; } = new List<ProjTask>(); 
}
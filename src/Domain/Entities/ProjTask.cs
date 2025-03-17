namespace Domain.Entities;

public class ProjTask
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public Project Project { get; set; }
}
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
    public List<Project?> Projects { get; set; }= [];
    public List<Role> Role { get; set; }= [];
}
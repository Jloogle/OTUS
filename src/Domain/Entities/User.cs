namespace Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string PhoneNumber { get; set; }
    public string email { get; set; }
    
    public long TelegramID { get; set; }
    public List<Project> Projects { get; set; }= new List<Project>();
    public List<Role> Role { get; set; }= new List<Role>();
}
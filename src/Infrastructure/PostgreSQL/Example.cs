using Application.Dto;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSQL;

public class Example
{
    //Пример добавления данных
    public static void Add()
    {
        using (ApplicationContext db = new ApplicationContext())
        {
            // создаем два проекта
            var Project1 = new Project { Name = "OTUS Project1", Deadline = DateTime.UtcNow };
            var Project2 = new Project { Name = "OTUS Project2", Deadline = DateTime.UtcNow };
            
            // добавляем их в бд
            db.Projects.AddRange(Project1, Project2);
            
            // создаем три пользователя
            var user1 = new User { Name = "Tom", Age = 33 , PhoneNumber = "+7 (916) 123 45 67", email = "Tom@gmail.com" };
            var user2 = new User { Name = "Alice", Age = 26, PhoneNumber = "+7 (916) 123 45 67", email = "Alice@gmail.com"};
            var user3 = new User { Name = "Bob", Age = 36, PhoneNumber = "+7 (916) 123 45 67", email = "Bob@gmail.com"};
            // добавляем их в бд
            db.Users.AddRange(user1, user2, user3);
            
            // добавляем проект к пользователю и пользователя к проекту(многие ко многим)
            user1.Projects.Add(Project1);
            user2.Projects.Add(Project1);
            user3.Projects.Add(Project1);
            user3.Projects.Add(Project2);
            Project2.Users.Add(user3);
            
            // создаем три роли
            Role role1 = new Role() { Name = "Admin" };
            Role role2 = new Role() { Name = "Manager" };
            Role role3 = new Role() { Name = "Executor" };
            
            // для ролей тоже многие ко многим
            user1.Role.Add(role1);
            user2.Role.Add(role2);
            user3.Role.Add(role3);
            role3.Users.Add(user2);
            
            // добавляем их в бд
            db.Roles.AddRange(role1, role2, role3);
            
            // создаем задачи
            var task1 = new ProjTask() { Name = "Task1" , Description = "description task1" };
            var task2 = new ProjTask() { Name = "Task2" , Description = "description task2" };
            var task3 = new ProjTask() { Name = "Task3" , Description = "description task3" };
            var task4 = new ProjTask() { Name = "Task4" , Description = "description task4" };
            var task5 = new ProjTask() { Name = "Task5" , Description = "description task5" };
            var task6 = new ProjTask() { Name = "Task6" , Description = "description task6" };
            
            Project1.ProjTasks.Add(task1);
            Project1.ProjTasks.Add(task2);
            Project1.ProjTasks.Add(task3);
            Project1.ProjTasks.Add(task4);
            Project2.ProjTasks.Add(task5);
            Project2.ProjTasks.Add(task6);
            
            // добавляем их в бд
            db.Tasks.AddRange(task1, task2, task3, task4, task5, task6);
            
            db.SaveChanges();
        }
    }

    public static void Print()
    {
        Add();
        using (ApplicationContext db = new ApplicationContext())
        {
 
            var UsersProjects = db.Users     
                .Include(s => s.Projects) 
                .ThenInclude(sc => sc.Users)  
                .ToList();
            
            // Вывод проектов пользователя
            foreach (var User in UsersProjects)
            {
                Console.WriteLine($"User Id: {User.Id}, Name: {User.Name}");
                
                if (User.Projects.Any())
                {
                    Console.WriteLine("Enrolled in the following projects:");
                    foreach (var Project in User.Projects)
                    {
                        Console.WriteLine($"\tProject Id:{Project.Id}, Name:{Project.Name}, Deadline: {Project.Deadline}");
                    }
                }
                else
                {
                    Console.WriteLine("No Project enrolled.");
                }
                Console.WriteLine(); 
            }
        }
        
        
    }
}
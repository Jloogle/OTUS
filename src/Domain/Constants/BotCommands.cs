using System.Reflection;
using Domain.Attributes;

namespace Domain.Constants;


public static class BotCommands
{
    [Commands("В начало.")]
    public const string Start = "/start";
    [Commands("Просмотр своих данных.")]
    public const string Profile = "/profile";
    [Commands("Help по функциям.")]
    public const string Help = "/help";
    [Commands("Начало работы с проектами.")]
    public const string Project = "/project";
    [Commands("Удаление проекта - /project_delete [Id проекта].")]
    public const string ProjectDelete = "/project_delete";
    [Commands("Создание проекта - /add_project [название проекта] [описание] [исполнитель] [дедлайн].")]
    public const string AddProject = "/add_project";
    [Commands("Вернуться в начало.")]
    public const string Back = "/back";
    [Commands("Показать мои проекты.")]
    public const string ListMyProjects = "/list_my_projects";
    [Commands("Показать мои задачи.")]
    public const string ListMyTasks = "/list_my_tasks";
    [Commands("Добавить или изменить задачу.")]
    public const string ChangeTask = "/change_task";
    


    public static List<string?> GetAllCommands()
    {
        return typeof(BotCommands)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(field => field is { IsLiteral: true, IsInitOnly: false })
            .Select(field => field.GetValue(null)?.ToString())
            .ToList();
    }
    
    public static bool IsValidCommand(string command)
    {
        var allCommands = GetAllCommands();
        return allCommands.Count != 0 && allCommands.Contains(command);
    }
    
    public static string? PrintHelpMenu()
    {
        var r = "Help information: ";
        
        var fields = typeof(BotCommands).GetFields();
        foreach (var field in fields)
        {
            var attrs = field.GetCustomAttributes(true);
            foreach (var attr in attrs)
            {
                if (attr is not CommandsAttribute authAttr) continue;
                
                var fieldValue = field.GetRawConstantValue()?.ToString();
                var desc = authAttr.GetDescription();
      
                r+=$"\n Команда: {fieldValue}: Описание: {desc}";
            }
        }

        return r;
    }
    
}


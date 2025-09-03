using System.Reflection;
using Domain.Attributes;

namespace Domain.Constants;


public static class BotCommands
{
    [Commands("В начало.")]
    public const string Start = "/start";
    [Commands("Регистрация пользователя (алиас /start).")]
    public const string Register = "/register";
    [Commands("Просмотр своих данных.")]
    public const string Profile = "/profile";
    [Commands("Help по функциям.")]
    public const string Help = "/help";
    [Commands("Начало работы с проектами.")]
    public const string Project = "/project";
    [Commands("Удаление проекта - /project_delete [Id проекта].")]
    public const string ProjectDelete = "/project_delete";
    [Commands("Создание проекта - /add_project [название проекта] [дедлайн].")]
    public const string AddProject = "/add_project";
    // Убрали алиас /project_create во избежание дублирования с /add_project
    [Commands("Вернуться в начало.")]
    public const string Back = "/back";
    [Commands("Показать мои проекты.")]
    public const string ListMyProjects = "/list_my_projects";
    [Commands("Показать мои задачи.")]
    public const string ListMyTasks = "/list_my_tasks";
    [Commands("Добавить или изменить задачу.")]
    public const string ChangeTask = "/change_task";
    [Commands("Создание задачи - /task_create [название] [описание] [ID проекта].")]
    public const string TaskCreate = "/task_create";
    [Commands("Пригласить в проект - /project_invite [ID проекта] [email] [роль?].")]
    public const string ProjectInvite = "/project_invite";

    [Commands("Принять приглашение в проект - /accept_invite [ID приглашения]")]
    public const string AcceptInvite = "/accept_invite";
    [Commands("Отклонить приглашение в проект - /decline_invite [ID приглашения]")]
    public const string DeclineInvite = "/decline_invite";

    // Invites listing
    public const string ListInvites = "/invites";
    public const string InviteHistory = "/invite_history";
    


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

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
    [Commands("Создание проекта - /project_create [название задачи] [проект] [исполнитель] [дедлайн].")]
    public const string ProjectCreate = "/project_create";
    [Commands("Удаление проекта - /project_delete [Id проекта].")]
    public const string ProjectDelete = "/project_delete";

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


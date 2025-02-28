using System.Reflection;

namespace Domain.Constants;

public static class BotCommands
{
    public const string Start = "/start";
    public const string Profile = "/profile";
    public const string Help = "/help";
    
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
}
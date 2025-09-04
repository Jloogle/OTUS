namespace Domain.Attributes;

/// <summary>
/// Атрибут для обозначения команды бота.
/// </summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class CommandsAttribute(string description) : Attribute
{
    public string GetDescription() => description;
}
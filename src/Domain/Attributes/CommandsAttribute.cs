namespace Domain.Attributes;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class CommandsAttribute(string description) : Attribute
{
    public string GetDescription() => description;
}
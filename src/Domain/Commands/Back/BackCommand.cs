using Domain.Commands;

namespace Domain.Commands.Back;

/// <summary>
/// Команда для возврата в начало.
/// </summary>
public class BackCommand : ICommand
{
    public long? UserId { get; set; }
    public string Command => "/back";
}

using Domain.Commands;

namespace Domain.Commands.Back;

public class BackCommand : ICommand
{
    public long? UserId { get; set; }
    public string Command => "/back";
}

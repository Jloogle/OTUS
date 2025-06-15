namespace Domain.Commands;

public interface ICommandHandler<TCommand>
    where TCommand : ICommand
{
    /// <summary>
    /// Возвращает сообщение пользователю
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<string?> Handle(TCommand command);
}
using Domain.Commands;

namespace Application.CommandRouting;

public interface ICommandRouting
{
    /// <summary>
    /// Возвращает сообщение пользователю
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<string?> RouteAsync(ICommand command);
}
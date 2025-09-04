namespace Domain.Commands;

/// <summary>
/// Маркер для команды бота, отправленной пользователем.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Строка-триггер команды, например "/start".
    /// </summary>
    string Command { get; }
}

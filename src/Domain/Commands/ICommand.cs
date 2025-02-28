namespace Domain.Commands;

public interface ICommand
{
    //Команда от пользователя, например, /start и т.д.
    string Command { get; }
}
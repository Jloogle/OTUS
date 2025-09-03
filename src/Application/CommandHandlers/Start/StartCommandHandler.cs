using System.Text.Json;
using Domain.Commands;
using Domain.Commands.Start;
using Domain.Entities;
using Domain.Repositories;
using System.Text.RegularExpressions;

namespace Application.CommandHandlers.Start;

public class StartCommandHandler : ICommandHandler<StartCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IRadisRepository _radisRepository;
    private string? _answer;
    private StartCommand? _command;

    public StartCommandHandler(IUserRepository userRepository, IRadisRepository radisRepository)
    {
        _userRepository = userRepository;
        _radisRepository = radisRepository;
    }

    public async Task<string?> Handle(StartCommand? command)
    {
        _command = command;
        var input = _command!.UserCommand;
        var isJustCommand = string.Equals(input, _command!.Command, StringComparison.OrdinalIgnoreCase);

        // Уже зарегистрирован в БД?
        var existing = await _userRepository.FindByIdTelegram(_command!.UserId);
        if (existing != null)
            return $"Вы уже зарегистрированы!\nВаше имя: {existing.Name}";

        var user = GetTempUser(_command!.UserId);
        user.IdTelegram = _command!.UserId;

        // Если пришла только команда (/start) или состояние не задано — показываем первый вопрос
        if (isJustCommand || user.State == (int)TrafficState.New)
        {
            user.State = (int)TrafficState.Name;
            SaveUserToRedis(user);
            return "Добро пожаловать!\nНеобходимо пройти процедуру регистрации!\nВведите ваше имя:";
        }

        switch ((TrafficState)user.State)
        {
            case TrafficState.Name:
                user.Name = input;
                user.State = (int)TrafficState.Email;
                SaveUserToRedis(user);
                return "Введите Email:";

            case TrafficState.Email:
                user.Email = input;
                user.State = (int)TrafficState.Phone;
                SaveUserToRedis(user);
                return "Введите телефон:";

            case TrafficState.Phone:
                user.PhoneNumber = input;
                user.State = (int)TrafficState.Age;
                SaveUserToRedis(user);
                return "Введите ваш возраст:";

            case TrafficState.Age:
                if (!int.TryParse(input, out var age) || age <= 0 || age > 120)
                {
                    return "Возраст должен быть числом от 1 до 120. Введите ваш возраст:";
                }
                user.Age = age;
                user.State = (int)TrafficState.Finished;
                SaveUserToRedis(user);
                try
                {
                    await _userRepository.AddUser(user);
                    _radisRepository.StringDelete("Reg: " + _command!.UserId);
                    return "Вы успешно зарегистрировались!";
                }
                catch
                {
                    return "Произошла ошибка при регистрации. Пожалуйста, попробуйте снова.";
                }

            case TrafficState.Finished:
                _radisRepository.StringDelete("Reg: " + _command!.UserId);
                return "Вы уже завершили регистрацию. Используйте /profile для просмотра данных.";
            default:
                user.State = (int)TrafficState.Name;
                SaveUserToRedis(user);
                return "Введите ваше имя:";
        }
    }

    private User GetTempUser(long? userId)
    {
        var jsonUser = _radisRepository.StringGet("Reg: " + userId);
        return !string.IsNullOrEmpty(jsonUser) 
            ? JsonSerializer.Deserialize<User>(jsonUser) ?? new User() 
            : new User();
    }

    private void SaveUserToRedis(User user)
    {
        _radisRepository.StringSet("Reg: " + _command!.UserId, JsonSerializer.Serialize(user));
    }
}
public enum TrafficState
{
    New,
    Name,
    Email,
    Phone,
    Age,
    Finished
}

public enum TrafficTrigger
{
    Go
}

using System.Text.Json;
using Domain.Commands;
using Domain.Commands.Start;
using Domain.Entities;
using Domain.Repositories;
using Stateless;

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

        var user = await _userRepository.FindByIdTelegram(command!.UserId);
        
        if (user != null)
            return $"Вы уже зарегистрированы!\nВаше имя: {user.Name}";
        
        var userRedis = GetTempUser(_command!.UserId);
        
        var stateMachine = ConfigStateMachine(userRedis.State);
        stateMachine.Fire(TrafficTrigger.Go);
            
        return _answer;
    }

    private StateMachine<TrafficState, TrafficTrigger> ConfigStateMachine(int state)
    {
        var stateMachine = new StateMachine<TrafficState, TrafficTrigger>((TrafficState)state);

        stateMachine.Configure(TrafficState.New)
            .Permit(TrafficTrigger.Go, TrafficState.Name);

        stateMachine.Configure(TrafficState.Name)
            .OnEntry(HandleNameState)
            .Permit(TrafficTrigger.Go, TrafficState.Email);
        
        stateMachine.Configure(TrafficState.Email)
            .OnEntry(HandleEmailState)
            .Permit(TrafficTrigger.Go, TrafficState.Phone);
        
        stateMachine.Configure(TrafficState.Phone)
            .OnEntry(HandlePhoneState)
            .Permit(TrafficTrigger.Go, TrafficState.Age);
        
        stateMachine.Configure(TrafficState.Age)
            .OnEntry(HandleAgeState)
            .Permit(TrafficTrigger.Go, TrafficState.Finished);
        
        stateMachine.Configure(TrafficState.Finished)
            .OnEntry(HandleFinishedState)
            .Permit(TrafficTrigger.Go, TrafficState.New);
        
        return stateMachine;
    }

    private void HandleNameState()
    {
        var user = GetTempUser(_command!.UserId);
        user.IdTelegram = _command!.UserId;
        user.State = (int)TrafficState.Name;
        if (_command!.UserCommand != _command!.Command)
            user.Name = _command!.UserCommand!;
        _answer = "Добро пожаловать!\nНеобходимо пройти процедуру регистрации!\nВведите ваше имя:";
        SaveUserToRedis(user);
    }

    private void HandleEmailState()
    {
        var user = GetTempUser(_command!.UserId);
        if (_command!.UserCommand != _command!.Command)
            user.Name = _command!.UserCommand!;
        user.State = (int)TrafficState.Email;
        SaveUserToRedis(user);
        _answer = "Введите Email:";
    }

    private void HandlePhoneState()
    {
        var user = GetTempUser(_command!.UserId);
        if (_command!.UserCommand != _command!.Command)
            user.Email = _command!.UserCommand!;
        user.State = (int)TrafficState.Phone;
        SaveUserToRedis(user);
        _answer = "Введите телефон:";
    }

    private void HandleAgeState()
    {
        var user = GetTempUser(_command!.UserId);
        if (_command!.UserCommand != _command!.Command)
            user.PhoneNumber = _command!.UserCommand!;
        user.State = (int)TrafficState.Age;
        SaveUserToRedis(user);
        _answer = "Введите ваш возраст:";
    }

    private async void HandleFinishedState()
    {
        var user = GetTempUser(_command!.UserId);
        try
        {
            if (_command!.UserCommand != _command!.Command)
                user.Age = int.Parse(_command!.UserCommand!);
            
            user.State = (int)TrafficState.Finished;
            _answer = "Вы успешно зарегистрировались!";
            SaveUserToRedis(user);
            await _userRepository.AddUser(user);
        }
        catch (Exception)
        {
            _answer = "Произошла ошибка при регистрации. Пожалуйста, попробуйте снова.";
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
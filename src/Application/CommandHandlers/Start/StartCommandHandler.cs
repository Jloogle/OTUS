using System.Text.Json;
using Domain.Commands;
using Domain.Commands.Start;
using Domain.Entities;
using Domain.Repositories;

using Stateless;

namespace Application.CommandHandlers.Start;

public class StartCommandHandler(IUserRepository userRepository, IRadisRepository radisRepository) : ICommandHandler<StartCommand>
{
    private StateMachine<TrafficState, TrafficTrigger>? _stateMachine;
    private string? _ansfer;
    private User? _user;
    private StartCommand? _command;
    
    public async Task<string?> Handle(StartCommand? command)
    {
        _command = command;
        
        if (_stateMachine is null)
        {
            ConfigStateMachine();
        }

        if (CurrentState == TrafficState.Finished)
        {
            _ansfer = "Вы уже зарегистрировались!";
        }
        else
        {
            Trigger(TrafficTrigger.Go);
        }
        return await Task.FromResult(_ansfer);
    }

    private void ConfigStateMachine()
    {
        SetRegUser();
        
        _stateMachine = new StateMachine<TrafficState, TrafficTrigger>(_user!.State == (int)TrafficState.New ? TrafficState.New : (TrafficState)_user!.State-1);
        // Define the state transitions
        _stateMachine.Configure(TrafficState.New)
            .Permit(TrafficTrigger.Go, TrafficState.Name);

        _stateMachine.Configure(TrafficState.Name)
            .OnEntry(() =>
            {
                _user.State = (int)CurrentState;
                _user!.Name = _command!.UserCommand!;
                _ansfer = "Добро пожаловать!\nНеобходимо пройти процедуру регистрации!\nВведите ваше имя:";
                radisRepository.StringSet("Reg: "+_command!.UserId, JsonSerializer.Serialize(_user));
            })
            .Permit(TrafficTrigger.Go, TrafficState.Email);
        
        _stateMachine.Configure(TrafficState.Email)
            .OnEntry(() =>
            {
                _user!.Name = _command!.UserCommand!;
                _user.State = (int)CurrentState;
                radisRepository.StringSet("Reg: "+_command.UserId, JsonSerializer.Serialize(_user));
                _ansfer = "Введите Email:";
                
            })
            .Permit(TrafficTrigger.Go, TrafficState.Phone);
        
        _stateMachine.Configure(TrafficState.Phone)
            .OnEntry(() =>
            {
                _user!.email = _command!.UserCommand!;
                _user.State = (int)CurrentState;
                radisRepository.StringSet("Reg: "+_command.UserId, JsonSerializer.Serialize(_user));
                _ansfer = "Введите телефон:";
                
            })
            .Permit(TrafficTrigger.Go, TrafficState.Age);
        
        _stateMachine.Configure(TrafficState.Age)
            .OnEntry(() =>
            {
                _user!.PhoneNumber = _command!.UserCommand!;
                _user.State = (int)CurrentState;
                radisRepository.StringSet("Reg: "+_command.UserId, JsonSerializer.Serialize(_user));
                _ansfer = "Введите ваш возраст:";
                
            })
            .Permit(TrafficTrigger.Go, TrafficState.Finished);
        
        _stateMachine.Configure(TrafficState.Finished)
            .OnEntry(async void () =>
            {
                try
                {
                    _user.Age = int.Parse(_command!.UserCommand!);
                    _user.State = (int)CurrentState;
                    _ansfer = "Вы успешно зарегистрировались!";
                    radisRepository.StringSet("Reg: "+_command.UserId, JsonSerializer.Serialize(_user));
                    await userRepository.AddUser(_user!);
                    
                }
                catch (Exception e)
                {
                    throw; // TODO handle exception
                }
            });
        
    }

    public void SetRegUser()
    {
        var jsonUser = radisRepository.StringGet("Reg: " + _command!.UserId);

        if (jsonUser != "")
            _user = JsonSerializer.Deserialize<User>(jsonUser);
        else 
            _user = null ?? new User();
    }


    private TrafficState CurrentState => _stateMachine!.State;

    private void Trigger(TrafficTrigger trigger)
    {
        _stateMachine!.Fire(trigger);
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
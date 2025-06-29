using System.Text.Json;
using Domain.Commands;
using Domain.Commands.Start;
using Domain.Entities;
using Domain.Repositories;
using Stateless;

namespace Application.CommandHandlers.Start;

public class StartCommandHandler(IUserRepository userRepository, IRadisRepository radisRepository) : ICommandHandler<StartCommand>
{
    //public StateMachine<TrafficState, TrafficTrigger>? StateMachine;
    private string? _ansfer;
    private StartCommand? _command;
    
    public async Task<string?> Handle(StartCommand? command)
    {
        _command = command;

       var user = await userRepository.FindByIdTelegram(command!.UserId);
        
        if (user != null)
            return $"Вы уже зарегистрированы!\nВаше имя: {user.Name}";
        
        var userRedis = GetTempUser(_command!.UserId);
        
        var l = new Lock();
        lock (l)
        {
           var sm = ConfigStateMachine(userRedis.State);
           Trigger(TrafficTrigger.Go, sm); 
        }
            
        return await Task.FromResult(_ansfer);
    }

    private StateMachine<TrafficState, TrafficTrigger> ConfigStateMachine(int state)
    {
        var stateMachine = new StateMachine<TrafficState, TrafficTrigger>(state == (int)TrafficState.New ? TrafficState.New : (TrafficState)state);
        // Define the state transitions
        stateMachine.Configure(TrafficState.New)
            .Permit(TrafficTrigger.Go, TrafficState.Name);

        stateMachine.Configure(TrafficState.Name)
            .OnEntry(() =>
            {
                var user = GetTempUser(_command!.UserId);
                user.IdTelegram=_command!.UserId;
                user.State = (int)CurrentState(stateMachine);
                if (_command!.UserCommand != _command!.Command)
                    user.Name = _command!.UserCommand!;
                _ansfer = "Добро пожаловать!\nНеобходимо пройти процедуру регистрации!\nВведите ваше имя:";
                radisRepository.StringSet("Reg: "+_command!.UserId, JsonSerializer.Serialize(user));
            })
            .Permit(TrafficTrigger.Go, TrafficState.Email);
        
        stateMachine.Configure(TrafficState.Email)
            .OnEntry(() =>
            {
                var user = GetTempUser(_command!.UserId);
                
                if (_command!.UserCommand != _command!.Command)
                    user.Name = _command!.UserCommand!;
        
                user.State = (int)CurrentState(stateMachine);
                radisRepository.StringSet("Reg: "+_command.UserId, JsonSerializer.Serialize(user));
                _ansfer = "Введите Email:";
                
            })
            .Permit(TrafficTrigger.Go, TrafficState.Phone);
        
        stateMachine.Configure(TrafficState.Phone)
            .OnEntry(() =>
            {
                var user = GetTempUser(_command!.UserId);
                if (_command!.UserCommand != _command!.Command)
                    user.Email = _command!.UserCommand!;
                user.State = (int)CurrentState(stateMachine);
                radisRepository.StringSet("Reg: "+_command.UserId, JsonSerializer.Serialize(user));
                _ansfer = "Введите телефон:";
                
            })
            .Permit(TrafficTrigger.Go, TrafficState.Age);
        
        stateMachine.Configure(TrafficState.Age)
            .OnEntry(() =>
            {
                var user = GetTempUser(_command!.UserId);
                if (_command!.UserCommand != _command!.Command)
                    user.PhoneNumber = _command!.UserCommand!;
                user.State = (int)CurrentState(stateMachine);
                radisRepository.StringSet("Reg: "+_command.UserId, JsonSerializer.Serialize(user));
                _ansfer = "Введите ваш возраст:";
                
            })
            .Permit(TrafficTrigger.Go, TrafficState.Finished);
        
        stateMachine.Configure(TrafficState.Finished)
            .OnEntry(async void () =>
            {
                var user = GetTempUser(_command!.UserId);
                try
                {
                    if (_command!.UserCommand != _command!.Command)
                        user.Age = int.Parse(_command!.UserCommand!);
                    
                    user.State = (int)CurrentState(stateMachine);
                    _ansfer = "Вы успешно зарегистрировались!";
                    radisRepository.StringSet("Reg: "+_command.UserId, JsonSerializer.Serialize(user));
                    await userRepository.AddUser(user);
                    
                }
                catch (Exception e)
                {
                    throw; // TODO handle exception
                }
            })
            .Permit(TrafficTrigger.Go, TrafficState.New);
        
        return stateMachine;
    }

    private User GetTempUser(long? userId)
    {
        var jsonUser = radisRepository.StringGet("Reg: " + userId);

        if (jsonUser != "")
            return JsonSerializer.Deserialize<User>(jsonUser) ?? new User();
      
        return new User();
    }


    private static TrafficState CurrentState(StateMachine<TrafficState, TrafficTrigger> sm) => sm.State;

    private static void Trigger(TrafficTrigger trigger, StateMachine<TrafficState, TrafficTrigger> sm)
    {
        sm.Fire(trigger);
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
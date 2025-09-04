using Domain.Commands;
using Domain.Commands.Back;
using Domain.Repositories;

namespace Application.CommandHandlers.Back;

/// <summary>
/// Очищает временное состояние пользователя и возвращает в главное меню.
/// </summary>
public class BackCommandHandler : ICommandHandler<BackCommand>
{
    private readonly IRadisRepository _radisRepository;

    public BackCommandHandler(IRadisRepository radisRepository)
    {
        _radisRepository = radisRepository;
    }

    /// <summary>
    /// Удаляет из Redis состояние регистрации/сессии для текущего пользователя.
    /// </summary>
    public async Task<string?> Handle(BackCommand? command)
    {
        // Очищаем состояние пользователя в Redis
        _radisRepository.StringDelete("Reg: " + command!.UserId);
        
        return "Вы вернулись в главное меню!";
    }
}

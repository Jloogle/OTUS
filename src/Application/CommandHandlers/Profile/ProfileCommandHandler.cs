using Domain.Commands;
using Domain.Commands.Profile;
using Domain.Repositories;

namespace Application.CommandHandlers.Profile;

/// <summary>
/// Показывает текущий профиль пользователя с ролями и проектами.
/// </summary>
public class ProfileCommandHandler(IUserRepository userRepository) : ICommandHandler<ProfileCommand>
{
    /// <summary>
    /// Получает пользователя по Telegram id и форматирует профиль для отображения.
    /// </summary>
    public async Task<string?> Handle(ProfileCommand command)
    {
        if (command.UserId is null)
            return "Не удалось определить пользователя.";

        // Пытаемся получить пользователя; если схема/БД не совпадает — даём дружелюбный ответ
        Domain.Entities.User? user = null;
        try
        {
            user = await userRepository.FindByIdTelegramWithDetails(command.UserId);
        }
        catch
        {
            return "Не удалось получить профиль. Похоже, вы ещё не зарегистрированы. Отправьте /start для регистрации.";
        }

        if (user is null)
        {
            return "Вы еще не зарегистрированы. Введите /start для регистрации.";
        }

        var roles = user.Roles?.Select(r => r.Name).Where(n => !string.IsNullOrWhiteSpace(n)).ToList() ?? new List<string>();
        var projects = user.Projects?.Select(p => p.Name).Where(n => !string.IsNullOrWhiteSpace(n)).ToList() ?? new List<string>();

        var lines = new List<string>
        {
            "Ваш профиль:",
            $"Имя: {user.Name ?? "—"}",
            $"Возраст: {(user.Age == 0 ? "—" : user.Age)}",
            $"Телефон: {user.PhoneNumber ?? "—"}",
            $"Email: {user.Email ?? "—"}",
            roles.Count > 0 ? $"Роли: {string.Join(", ", roles)}" : "Роли: —",
            projects.Count > 0 ? $"Проекты: {string.Join(", ", projects)}" : "Проекты: —"
        };

        lines.Add("\nДля обновления данных используйте /start и пройдите обновление.");

        return string.Join("\n", lines);
    }
}

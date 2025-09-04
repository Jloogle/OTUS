using System.Text;
using Domain.Commands;
using Domain.Commands.Project;
using Domain.Repositories;
using Domain.Constants;

public class ListProjectCommandHandler(
    IProjectRepository projectRepository,
    IUserRepository userRepository
) : ICommandHandler<ListProjectCommand>
{
    /// <summary>
    /// Выводит список проектов текущего пользователя и контекстные команды бота.
    /// </summary>
    public async Task<string?> Handle(ListProjectCommand command)
    {
        if (command.UserId == null)
            return "Ошибка: не удалось определить пользователя.";

        // Найти пользователя по Telegram ID
        var user = await userRepository.FindByIdTelegram(command.UserId.Value);
        if (user == null)
            return "Пользователь не найден. Сначала зарегистрируйтесь командой /start";

        Console.WriteLine($"Найден пользователь: ID={user.Id}, Name={user.Name}, TelegramId={user.IdTelegram}");

        // Получить проекты пользователя
        var projects = await projectRepository.GetUserProjectsAsync(user.Id);

        Console.WriteLine($"Найдено проектов: {projects?.Count() ?? 0}");

        if (projects == null || !projects.Any())
            return "У вас пока нет проектов. Создайте проект командой /add_project";

        var result = new StringBuilder();
        result.AppendLine("📋 ВАШИ ПРОЕКТЫ:");
        result.AppendLine(new string('=', 50));
        result.AppendLine();

        foreach (var project in projects)
        {
            result.AppendLine($"🆔 ID: {project.Id}");
            result.AppendLine($"📝 Название: {project.Name}");
            result.AppendLine($"📅 Дедлайн: {project.Deadline:dd.MM.yyyy}");
            result.AppendLine();
            result.AppendLine("💡 КОМАНДЫ ДЛЯ УПРАВЛЕНИЯ:");
            result.AppendLine($"   • Пригласить участника: {BotCommands.ProjectInvite} [{project.Id}] [email] [роль?]");
            result.AppendLine($"   • Создать задачу: {BotCommands.TaskCreate} [название] [описание] {project.Id}");
            result.AppendLine($"   • Удалить проект: {BotCommands.ProjectDelete} [{project.Id}]");
            result.AppendLine();
            result.AppendLine(new string('-', 50));
            result.AppendLine();
        }

        result.AppendLine("📌 ДОПОЛНИТЕЛЬНЫЕ КОМАНДЫ:");
        result.AppendLine($"   • Создать новый проект: {BotCommands.AddProject}");
        result.AppendLine($"   • Мои задачи: {BotCommands.ListMyTasks}");
        result.AppendLine($"   • В начало: {BotCommands.Start}");

        return result.ToString();
    }
}

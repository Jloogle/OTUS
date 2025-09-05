//
// using System.Text.RegularExpressions;
// using Domain.Commands;
// using Domain.Commands.Task;
// using Domain.Entities;
// using Domain.Repositories;
// using Domain.Services;
// using Telegram.Bot;
//
// namespace Application.CommandHandlers.ProjectTask;
//
// /// <summary>
// /// Обрабатывает операции с задачами (CRUD-подобные) через одну команду /change_task.
// /// </summary>
// public class ChangeTaskCommandHandler(ITaskRepository taskRepository, IUserRepository userRepository, ITaskInviteStore taskInviteStore, ITelegramBotClient bot) : ICommandHandler<ChangeTaskCommand>
// {
//     /// <summary>
//     /// Делегирует выполнение конкретному действию на основе разобранных аргументов.
//     /// </summary>
//     public async Task<string?> Handle(ChangeTaskCommand command)
//     {
//         var parsedCommand = ParseChangeTaskCommand(command.UserCommand);
//         if (parsedCommand.Length == 0)
//         {
//             return GetHelpMessage();
//         }
//
//         var action = parsedCommand[0].ToLower();
//         
//         return action switch
//         {
//             "add" when parsedCommand.Length == 4 => await HandleAddTask(parsedCommand),
//             "del" when parsedCommand.Length == 2 => await HandleDeleteTask(parsedCommand),
//             "assign" when parsedCommand.Length == 3 => await HandleAssignTask(parsedCommand),
//             "assign_email" when parsedCommand.Length == 3 => await HandleAssignTaskByEmail(parsedCommand),
//             "list" when parsedCommand.Length == 1 => await HandleListTasks(),
//             "my" when parsedCommand.Length == 1 => await HandleMyTasks(command.UserId),
//             _ when parsedCommand.Length == 3 && int.TryParse(parsedCommand[0], out _) => await HandleUpdateTask(parsedCommand),
//             _ => GetHelpMessage()
//         };
//     }
//
//     /// <summary>Добавляет новую задачу в проект.</summary>
//     private async Task<string> HandleAddTask(string[] parsedCommand)
//     {
//         var taskName = parsedCommand[1];
//         var description = parsedCommand[2];
//         
//         if (string.IsNullOrWhiteSpace(description))
//         {
//             return "Описание задачи не может быть пустым.";
//         }
//
//         if (!int.TryParse(parsedCommand[3], out var projectId))
//         {
//             return "Неверный формат ID проекта.";
//         }
//
//         var task = new ProjTask { Name = taskName, Description = description };
//         await taskRepository.AddTaskAsync(task, projectId);
//         return $"Задача '{taskName}' успешно добавлена.";
//     }
//
//     /// <summary>Удаляет задачу по ID.</summary>
//     private async Task<string> HandleDeleteTask(string[] parsedCommand)
//     {
//         if (!int.TryParse(parsedCommand[1], out var taskId))
//         {
//             return "Неверный формат ID задачи.";
//         }
//
//         await taskRepository.DeleteTaskAsync(taskId);
//         return $"Задача с ID {taskId} успешно удалена.";
//     }
//
//     /// <summary>Назначает задачу пользователю.</summary>
//     private async Task<string> HandleAssignTask(string[] parsedCommand)
//     {
//         if (!int.TryParse(parsedCommand[1], out var taskId) || !int.TryParse(parsedCommand[2], out var userId))
//         {
//             return "Неверный формат ID задачи или пользователя.";
//         }
//
//         await taskRepository.AssignTaskToUserAsync(taskId, userId);
//         return $"Пользователь с ID {userId} успешно назначен на задачу с ID {taskId}.";
//     }
//
//     private async Task<string> HandleAssignTaskByEmail(string[] parsedCommand)
//     {
//         if (!int.TryParse(parsedCommand[1], out var taskId))
//         {
//             return "Неверный формат ID задачи.";
//         }
//         var email = parsedCommand[2];
//         var user = await userRepository.FindByEmailAsync(email);
//         if (user == null)
//         {
//             return $"Пользователь с email {email} не найден.";
//         }
//         if (user.IdTelegram is null)
//         {
//             return "У пользователя нет Telegram ID для отправки приглашения.";
//         }
//         // Создаём инвайт на задачу и отправляем кнопки
//         var inviteId = await taskInviteStore.CreateAsync(taskId, user.IdTelegram.Value);
//         var keyboard = new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup
//         {
//             Keyboard =
//             [
//                 [new Telegram.Bot.Types.ReplyMarkups.KeyboardButton($"/accept_task [{inviteId}]")],
//                 [new Telegram.Bot.Types.ReplyMarkups.KeyboardButton($"/decline_task [{inviteId}]")]
//             ],
//             ResizeKeyboard = true
//         };
//         await bot.SendMessage(user.IdTelegram.Value, $"Вам предлагается задача {taskId}.", replyMarkup: keyboard);
//         return $"Приглашение на задачу отправлено пользователю {email}. ID: {inviteId}";
//     }
//
//     /// <summary>Выводит все задачи.</summary>
//     private async Task<string> HandleListTasks()
//     {
//         var allTasks = await taskRepository.GetAllTasksAsync();
//         if (!allTasks.Any())
//         {
//             return "Задачи не найдены.";
//         }
//
//         var taskList = string.Join("\n", allTasks.Select(t => 
//             $"ID: {t.Id}, Название: {t.Name}, Описание: {t.Description}, Назначен: {(t.AssignedUsers.FirstOrDefault()?.Name!.ToString() ?? "Не назначен")}"));
//         return $"Все задачи:\n{taskList}";
//     }
//
//     /// <summary>Показывает задачи, назначенные текущему пользователю.</summary>
//     private async Task<string> HandleMyTasks(long? commandUserId)
//     {
//
//
//         var userTasks = await taskRepository.GetTasksByUserIdAsync(commandUserId);
//         if (!userTasks.Any())
//         {
//             return "У вас нет назначенных задач.";
//         }
//
//         var taskList = string.Join("\n", userTasks.Select(t => 
//             $"ID: {t.Id}, Название: {t.Name}, Описание: {t.Description}"));
//         return $"Ваши задачи:\n{taskList}";
//     }
//
//     /// <summary>Обновляет название и описание задачи.</summary>
//     private async Task<string> HandleUpdateTask(string[] parsedCommand)
//     {
//         if (!int.TryParse(parsedCommand[0], out var taskId))
//         {
//             return "Неверный формат ID задачи.";
//         }
//
//         var newTaskName = parsedCommand[1];
//         var newDescription = parsedCommand[2];
//
//         var projTask = await taskRepository.GetByIdAsync(taskId);
//         if (projTask == null)
//         {
//             return $"Задача с ID {taskId} не найдена.";
//         }
//
//         projTask.Name = newTaskName;
//         projTask.Description = newDescription;
//         
//         await taskRepository.UpdateTaskAsync(projTask);
//         return $"Задача с ID {taskId} успешно обновлена. Новое название: '{newTaskName}'.";
//     }
//
//     private static string GetHelpMessage()
//     {
//         return "Управление задачами:\n" +
//                "- Добавить: /change_task [add] [название задачи] [описание] [ID проекта]\n" +
//                "- Удалить: /change_task [del] [ID задачи]\n" +
//                "- Назначить по ID: /change_task [assign] [ID задачи] [ID пользователя]\n" +
//                "- Назначить по email: /change_task [assign_email] [ID задачи] [email]\n" +
//                "- Изменить: /change_task [ID задачи] [новое название] [новое описание]\n" +
//                "- Все задачи: /change_task [list]\n" +
//                "- Мои задачи: /change_task [my]";
//     }
//
//     /// <summary>
//     /// Извлекает токены из ввода команды /change_task в квадратных скобках.
//     /// </summary>
//     private string[] ParseChangeTaskCommand(string userCommand)
//     {
//         var cleanedCommand = Regex.Replace(userCommand, @"^/change_task\s*", "", RegexOptions.IgnoreCase).Trim();
//         var matches = Regex.Matches(cleanedCommand, @"\[(.*?)\]");
//         return matches.Select(m => m.Groups[1].Value.Trim()).ToArray();
//     }
// }

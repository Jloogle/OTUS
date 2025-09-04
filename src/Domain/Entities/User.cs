using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// Пользователь приложения с контактными данными, ролями и участием в проектах/задачах.
/// </summary>
public class User
{
    /// <summary>Первичный ключ.</summary>
    public int Id { get; init; }
    /// <summary>Отображаемое имя пользователя.</summary>
    public string? Name { get; set; }
    /// <summary>Возраст пользователя в годах.</summary>
    public int Age { get; set; }
    /// <summary>Номер телефона для связи.</summary>
    public string? PhoneNumber { get; set; }
    /// <summary>Адрес электронной почты для уведомлений.</summary>
    public string? Email { get; set; }
    [NotMapped]
    /// <summary>Переходное состояние потока регистрации (не хранится в БД).</summary>
    public int State { get; set; }
    /// <summary>Идентификатор пользователя Telegram (chat id).</summary>
    public long? IdTelegram { get; set; }

    // Связи с другими сущностями
    /// <summary>Проекты, в которых участвует пользователь.</summary>
    public List<Project> Projects { get; set; } = new List<Project>();

    /// <summary>Задачи, назначенные пользователю для выполнения.</summary>
    public List<ProjTask> AssignedTasks { get; set; } = new List<ProjTask>();

    /// <summary>Роли пользователя в системе безопасности.</summary>
    public List<Role> Roles { get; set; } = new List<Role>();
}

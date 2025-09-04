namespace Domain.Entities;

/// <summary>
/// Роль безопасности, предоставляющая права пользователям.
/// </summary>
public class Role
{
    /// <summary>Уникальный идентификатор роли.</summary>
    public int Id { get; set; }
    /// <summary>Название роли для идентификации прав доступа.</summary>
    public string Name { get; set; }
    /// <summary>Список пользователей, имеющих данную роль.</summary>
    public List<User> Users { get; set; } = new List<User>();
}

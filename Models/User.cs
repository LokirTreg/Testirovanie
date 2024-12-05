namespace Models;

public class User
{
    public int? Id { get; set; } // Уникальный идентификатор пользователя
    public string? Username { get; set; } // Имя пользователя
    public string? Email { get; set; } // Электронная почта
    public string? PasswordHash { get; set; } // Хэш пароля
    //public Role? UserRole { get; set; } // Роль пользователя (студент, администратор и т.д.)
}

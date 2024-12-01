namespace DataAccessService.Models;

public class TestAdministrator
{
    public int Id { get; set; } // Уникальный идентификатор администратора
    public int UserId { get; set; } // Идентификатор пользователя, который является администратором

    public virtual User User { get; set; } // Связь с пользователем-администратором

    // Методы для управления тестами могут быть добавлены здесь.
}
